"""
Smoke test end-to-end de la API SYNC.

Uso:
  python smoke_test.py --user http://localhost:7021/apigateway --maint http://localhost:7021/apigateway \
                       --root-user root --root-pass '...'
  (sin gateway: --user http://localhost:5101/api --maint http://localhost:5102/api)

Verifica: login, CRUD, ubicación de equipos con historial, árbol de guía (Proced/Valid),
aislamiento entre compañías (tenant), permisos por rol y respuestas de error (ProblemDetails).
Crea datos con sufijo aleatorio; no borra nada de lo existente.
"""
import argparse
import json
import sys
import urllib.error
import urllib.request
import uuid

FALLOS = []


def call(method, url, token=None, body=None):
    data = None if body is None else json.dumps(body).encode()
    req = urllib.request.Request(url, data=data, method=method)
    req.add_header("Content-Type", "application/json")
    if token:
        req.add_header("Authorization", f"Bearer {token}")
    try:
        with urllib.request.urlopen(req, timeout=30) as r:
            raw = r.read()
            return r.status, (json.loads(raw) if raw else None)
    except urllib.error.HTTPError as e:
        raw = e.read()
        try:
            return e.code, json.loads(raw) if raw else None
        except ValueError:
            return e.code, raw.decode(errors="replace")


def upload(url, token, campos, archivo, contenido, content_type="image/png"):
    """POST multipart/form-data con un archivo (campo 'archivo')."""
    limite = uuid.uuid4().hex
    partes = []
    for k, v in campos.items():
        partes.append(f'--{limite}\r\nContent-Disposition: form-data; name="{k}"\r\n\r\n{v}\r\n'.encode())
    partes.append((f'--{limite}\r\nContent-Disposition: form-data; name="archivo"; filename="{archivo}"\r\n'
                   f"Content-Type: {content_type}\r\n\r\n").encode() + contenido + b"\r\n")
    partes.append(f"--{limite}--\r\n".encode())
    req = urllib.request.Request(url, data=b"".join(partes), method="POST")
    req.add_header("Content-Type", f"multipart/form-data; boundary={limite}")
    req.add_header("Authorization", f"Bearer {token}")
    try:
        with urllib.request.urlopen(req, timeout=30) as r:
            return r.status, json.loads(r.read())
    except urllib.error.HTTPError as e:
        raw = e.read()
        try:
            return e.code, json.loads(raw) if raw else None
        except ValueError:
            return e.code, raw.decode(errors="replace")


def download(url, token):
    req = urllib.request.Request(url, headers={"Authorization": f"Bearer {token}"})
    try:
        with urllib.request.urlopen(req, timeout=30) as r:
            return r.status, r.headers.get("Content-Type"), r.read()
    except urllib.error.HTTPError as e:
        return e.code, None, e.read()


# PNG 1x1 válido
PNG = bytes.fromhex("89504e470d0a1a0a0000000d4948445200000001000000010806000000"
                    "1f15c4890000000d49444154789c6360000002000154a24f5d0000000049454e44ae426082")


def check(nombre, cond, detalle=""):
    print(("  OK   " if cond else "  FAIL ") + nombre + ("" if cond else f"  -> {detalle}"))
    if not cond:
        FALLOS.append(nombre)


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("--user", required=True)
    ap.add_argument("--maint", required=True)
    ap.add_argument("--root-user", required=True)
    ap.add_argument("--root-pass", required=True)
    a = ap.parse_args()
    U, M = a.user.rstrip("/"), a.maint.rstrip("/")
    sfx = uuid.uuid4().hex[:6]

    print("== Autenticación")
    s, b = call("POST", f"{U}/auth", body={"usuario": a.root_user, "contrasenia": "mala"})
    check("login con contraseña incorrecta -> 401", s == 401, s)
    s, b = call("POST", f"{U}/auth", body={"usuario": a.root_user[:-1], "contrasenia": a.root_pass})
    check("login con usuario parcial (LIKE) -> 401", s == 401, s)
    s, b = call("POST", f"{U}/auth", body={"usuario": a.root_user, "contrasenia": a.root_pass})
    check("login Root -> 200 con token", s == 200 and b and b.get("token"), (s, b))
    root = b["token"]
    s, me = call("GET", f"{U}/auth/me", root)
    check("GET /auth/me", s == 200 and me["rol"] == "Root", (s, me))
    s, _ = call("GET", f"{M}/equipo")
    check("sin token -> 401", s == 401, s)

    print("== Compañías (Root)")
    s, b = call("POST", f"{M}/comp", root, {"nombre": f"Comp A {sfx}", "nit": "900", "ciudad": "Bogotá"})
    check("crear compañía A -> 201", s == 201 and b.get("id"), (s, b))
    compA = b["id"]
    s, b = call("POST", f"{M}/comp", root, {"nombre": f"Comp B {sfx}", "nit": "901", "ciudad": "Cali"})
    compB = b["id"]

    print("== Ubicaciones (compañía A)")
    s, b = call("POST", f"{M}/planta", root, {"nombre": "Planta Norte", "region": "Andina", "idComp": compA})
    check("crear planta", s == 201, (s, b))
    planta = b["id"]
    s, tipos = call("GET", f"{M}/tipoarea", root)
    check("tipos de área sembrados (Produccion/Bodega/Patio)", s == 200 and len(tipos) >= 3, (s, tipos))
    prod = next(t["id"] for t in tipos if t["nombre"] == "Produccion")
    bod = next(t["id"] for t in tipos if t["nombre"] == "Bodega")
    s, b = call("POST", f"{M}/area", root, {"idPlanta": planta, "nombre": "Línea 1", "tipoArea": prod})
    areaProd = b["id"]
    s, b = call("POST", f"{M}/area", root, {"idPlanta": planta, "nombre": "Bodega central", "tipoArea": bod})
    areaBod = b["id"]
    s, b = call("POST", f"{M}/seccionarea", root, {"idArea": areaProd, "nombre": "Celda 3"})
    check("crear sección", s == 201, (s, b))
    secProd = b["id"]
    s, b = call("POST", f"{M}/seccionarea", root, {"idArea": areaBod, "nombre": "Estante A"})
    secBod = b["id"]
    s, b = call("GET", f"{M}/seccionarea?IdPlanta={planta}", root)
    check("listar secciones por planta (2)", s == 200 and len(b) == 2, (s, b))

    print("== Equipos y ubicación")
    eq = {"nombre": "Bomba P-101", "idComp": compA, "marca": "KSB", "nSerie": f"S-{sfx}", "anioFabricacion": 2019,
          "potencia": 15, "ubicacion": {"tipoUbicacion": "Produccion", "idSeccionArea": secProd}}
    s, b = call("POST", f"{M}/equipo", root, eq)
    check("crear equipo con ubicación", s == 201, (s, b))
    equipo = b["id"]
    s, b = call("GET", f"{M}/equipo/{equipo}", root)
    check("equipo devuelve ubicación vigente", s == 200 and b["ubicacion"] and b["ubicacion"]["seccionNombre"] == "Celda 3", (s, b))
    eq["ubicacion"] = {"tipoUbicacion": "Almacen", "idSeccionArea": secBod}
    s, b = call("PUT", f"{M}/equipo/{equipo}", root, eq)
    check("mover equipo a bodega", s == 200, (s, b))
    s, b = call("GET", f"{M}/equipo/{equipo}/ubicaciones", root)
    check("historial de ubicaciones = 2", s == 200 and len(b) == 2 and b[0]["vigente"], (s, b))
    s, b = call("POST", f"{M}/equipo", root, {"nombre": "X", "idComp": compA, "ubicacion": {"tipoUbicacion": "Bodega"}})
    check("tipoUbicacion inválido -> 400", s == 400, (s, b))
    s, b = call("POST", f"{M}/equipo", root, {"nombre": "Sin ubicación", "idComp": compA})
    check("crear equipo sin ubicación (antes NullReference)", s == 201, (s, b))
    s, b = call("GET", f"{M}/equipo?IdComp={compB}", root)
    check("lista vacía -> 200 [] (antes 404)", s == 200 and b == [], (s, b))

    print("== Guía con árbol Proced/Valid")
    guia = {"nombre": "Mantenimiento bomba", "idComp": compA, "intervalo": 30, "personal": 2, "duracion": 3,
            "proced": [{"nombre": "Bloqueo LOTO", "valid": [{"nombre": "Candado puesto"}, {"nombre": "Energía cero"}]},
                       {"nombre": "Cambio de sello", "valid": [{"nombre": "Sin fugas"}]}]}
    s, b = call("POST", f"{M}/guia", root, guia)
    check("crear guía con 2 pasos / 3 validaciones", s == 201, (s, b))
    idGuia = b["id"]
    s, g = call("GET", f"{M}/guia/{idGuia}", root)
    check("detalle de guía trae el árbol", s == 200 and len(g["proced"]) == 2 and sum(len(p["valid"]) for p in g["proced"]) == 3, (s, g))
    g["proced"] = [g["proced"][0]]
    g["proced"][0]["valid"].append({"nombre": "Señalización"})
    s, b = call("PUT", f"{M}/guia/{idGuia}", root, g)
    s, g2 = call("GET", f"{M}/guia/{idGuia}", root)
    check("actualizar árbol (quita paso, agrega validación)", len(g2["proced"]) == 1 and len(g2["proced"][0]["valid"]) == 3, g2)
    s, b = call("POST", f"{M}/guiaequipo", root, {"idGuia": idGuia, "idEquipo": equipo})
    check("asignar guía a equipo", s == 201, (s, b))
    s, b = call("POST", f"{M}/guiaequipo", root, {"idGuia": idGuia, "idEquipo": equipo})
    check("asignación duplicada -> 409", s == 409, (s, b))

    print("== Actividades")
    s, b = call("POST", f"{M}/tipoactividad", root, {"nombre": "Preventiva", "idComp": compA})
    tipoAct = b["id"]
    s, b = call("POST", f"{U}/cuad", root, {"nombre": "Cuadrilla mecánica", "idComp": compA})
    check("crear cuadrilla", s == 201, (s, b))
    cuad = b["id"]
    s, b = call("POST", f"{M}/actividad", root, {"idComp": compA, "idTipoActividad": tipoAct, "descripcion": "PM bomba",
                                                  "fechaEjecucion": "2026-10-20T08:00:00", "idCuad": cuad, "intervalo": 30})
    check("crear actividad", s == 201, (s, b))
    act = b["id"]
    s, b = call("POST", f"{M}/actividadequipo", root, {"idActividad": act, "idEquipo": equipo, "idGuia": idGuia})
    check("agregar equipo a actividad", s == 201, (s, b))
    s, b = call("GET", f"{M}/actividad/{act}", root)
    check("actividad muestra cuadrilla y nº equipos", s == 200 and b["cuadNombre"] == "Cuadrilla mecánica" and b["numEquipos"] == 1, (s, b))

    print("== Operación en campo: ejecución, fotos y fallas")
    s, b = call("POST", f"{M}/guia", root, {"nombre": "Cambio de sello", "idComp": compA, "intervalo": 90, "personal": 1,
                                             "duracion": 2, "proced": [{"nombre": "Montaje", "valid": [
                                                 {"nombre": "Eje limpio"}, {"nombre": "Sello instalado", "requiereFoto": True}]}]})
    idGuiaFoto = b["id"]
    s, g = call("GET", f"{M}/guia/{idGuiaFoto}", root)
    v1, v2 = g["proced"][0]["valid"]
    check("la guía guarda 'requiere foto'", v2["requiereFoto"] is True and v1["requiereFoto"] is False, g)
    s, b = call("POST", f"{M}/equipo", root, {"nombre": "Bomba P-202", "idComp": compA,
                                               "ubicacion": {"tipoUbicacion": "Produccion", "idSeccionArea": secProd}})
    equipo2 = b["id"]
    s, b = call("POST", f"{M}/actividadequipo", root, {"idActividad": act, "idEquipo": equipo2, "idGuia": idGuiaFoto})
    ae = b["id"]
    s, b = call("GET", f"{M}/actividadequipo?IdCuad={cuad}&FechaDesde=2026-10-20&FechaHasta=2026-10-20", root)
    fila = next((x for x in b if x["id"] == ae), None) if s == 200 else None
    check("equipos del día por cuadrilla, con progreso y ubicación",
          fila and fila["estadoEjec"] == "Pendiente" and fila["totalValid"] == 2 and fila["ubicacionNombre"], (s, b))
    s, b = call("PUT", f"{M}/actividadequipo/{ae}/resultados/{v1['id']}", root, {"cumple": True})
    check("marcar verificación", s == 200, (s, b))
    s, b = call("GET", f"{M}/actividadequipo/{ae}", root)
    check("la primera marca inicia la ejecución", b["estadoEjec"] == "EnCurso" and b["validCumplidas"] == 1, b)
    s, b = call("PUT", f"{M}/actividadequipo/{ae}/resultados/no-existe", root, {"cumple": True})
    check("verificación ajena a la guía -> 404", s == 404, (s, b))
    s, b = call("POST", f"{M}/actividadequipo/{ae}/finalizar", root, {})
    check("finalizar con verificaciones pendientes -> 400", s == 400, (s, b))
    call("PUT", f"{M}/actividadequipo/{ae}/resultados/{v2['id']}", root, {"cumple": True})
    s, b = call("POST", f"{M}/actividadequipo/{ae}/finalizar", root, {})
    check("finalizar sin la foto que exige la guía -> 400", s == 400 and "foto" in str(b), (s, b))
    s, b = upload(f"{M}/adjunto", root, {"entidad": "Ejecucion", "idEntidad": ae, "ref": v2["id"]}, "sello.png", PNG)
    check("subir foto de evidencia", s == 201, (s, b))
    fotoEvidencia = b["id"]
    s, b = upload(f"{M}/adjunto", root, {"entidad": "Equipo", "idEntidad": equipo2}, "notas.txt", b"hola", "text/plain")
    check("adjunto que no es imagen -> 400", s == 400, (s, b))
    s, ctype, datos = download(f"{M}/adjunto/{fotoEvidencia}/archivo", root)
    check("descargar foto (mismos bytes)", s == 200 and ctype == "image/png" and datos == PNG, (s, ctype))
    s, b = call("POST", f"{M}/actividadequipo/{ae}/finalizar", root, {"observacion": "Sin novedad"})
    check("finalizar con checklist y fotos completos", s == 200, (s, b))
    s, b = call("PUT", f"{M}/actividadequipo/{ae}/resultados/{v1['id']}", root, {"cumple": False})
    check("ejecución finalizada no admite cambios -> 400", s == 400, (s, b))
    s, b = upload(f"{M}/adjunto", root, {"entidad": "Equipo", "idEntidad": equipo2}, "vista.png", PNG)
    check("subir foto del equipo", s == 201, (s, b))
    fotoEquipo = b["id"]
    s, b = call("GET", f"{M}/adjunto?entidad=Equipo&idEntidad={equipo2}", root)
    check("listar fotos del equipo", s == 200 and len(b) == 1 and b[0]["tamano"] == len(PNG), (s, b))
    s, b = call("POST", f"{M}/falla", root, {"idEquipo": equipo2, "tipo": "Fuga", "prioridad": "Critica",
                                              "equipoDetenido": True, "descripcion": "Goteo en el sello"})
    check("reportar falla", s == 201, (s, b))
    falla = b["id"]
    s, b = call("GET", f"{M}/falla/{falla}", root)
    check("la falla queda abierta y con el equipo", s == 200 and b["estadoFalla"] == "Abierta" and b["equipoNombre"] == "Bomba P-202", (s, b))
    s, b = call("POST", f"{M}/falla", root, {"idEquipo": equipo2, "tipo": "Explosion", "prioridad": "Alta"})
    check("tipo de falla inválido -> 400", s == 400, (s, b))

    print("== Turnos y horarios")
    s, b = call("POST", f"{U}/turno", root, {"nombre": "Mañana", "idComp": compA, "horaInicio": "06:00", "horaFin": "14:00"})
    check("crear turno", s == 201, (s, b))
    turno = b["id"]
    s, b = call("POST", f"{U}/horario", root, {"idTurno": turno, "idCuad": cuad, "diaSemana": 1})
    check("crear horario", s == 201, (s, b))
    s, b = call("POST", f"{U}/horario", root, {"idTurno": turno, "idCuad": cuad, "diaSemana": 9})
    check("díaSemana inválido -> 400", s == 400, (s, b))

    print("== Usuarios, credenciales y tenant")
    ident = int(uuid.uuid4().int % 10**9)
    s, b = call("POST", f"{U}/users", root, {"nombre": "Ana", "apellido": "Admin B", "identificacion": ident,
                                              "rol": "Admin", "idComp": compB})
    check("crear Admin de compañía B (sin cuadrilla: antes fallaba la FK)", s == 201, (s, b))
    adminB = b["id"]
    s, b = call("POST", f"{U}/credential", root, {"usuario": f"adminb{sfx}", "contrasenia": "corta", "idUser": adminB})
    check("contraseña corta -> 400", s == 400, (s, b))
    s, b = call("POST", f"{U}/credential", root, {"usuario": f"adminb{sfx}", "contrasenia": "Clave#Segura1", "idUser": adminB})
    check("crear credencial", s == 201, (s, b))
    s, b = call("POST", f"{U}/auth", body={"usuario": f"adminb{sfx}", "contrasenia": "Clave#Segura1"})
    check("login Admin B", s == 200, (s, b))
    tb = b["token"]

    s, b = call("GET", f"{M}/equipo?IdComp={compA}", tb)
    check("Admin B NO ve equipos de A aunque pida IdComp=A", s == 200 and all(e["idComp"] == compB for e in b), (s, b))
    s, b = call("GET", f"{M}/equipo/{equipo}", tb)
    check("Admin B no accede a equipo de A -> 404", s == 404, (s, b))
    s, b = call("PUT", f"{M}/equipo/{equipo}", tb, eq)
    check("Admin B no modifica equipo de A -> 404", s == 404, (s, b))
    s, b = call("POST", f"{M}/equipo", tb, {"nombre": "Compresor B", "idComp": compA})
    check("Admin B crea equipo (forzado a su compañía)", s == 201, (s, b))
    s, b = call("GET", f"{M}/equipo/{b['id']}", tb)
    check("...y queda en la compañía B", s == 200 and b["idComp"] == compB, (s, b))
    s, b = call("POST", f"{M}/seccionarea", tb, {"idArea": areaProd, "nombre": "Intrusa"})
    check("Admin B no crea sección en área de A -> 404", s == 404, (s, b))
    s, b = call("POST", f"{U}/users", tb, {"nombre": "Eve", "identificacion": ident + 1, "rol": "Root"})
    check("Admin B no puede crear usuario Root -> 403", s == 403, (s, b))
    s, b = call("GET", f"{M}/comp", tb)
    check("Admin B solo ve su compañía", s == 200 and len(b) == 1 and b[0]["id"] == compB, (s, b))
    s, b = call("POST", f"{M}/comp", tb, {"nombre": "Hack"})
    check("Admin B no crea compañías -> 403", s == 403, (s, b))
    s, _, _ = download(f"{M}/adjunto/{fotoEquipo}/archivo", tb)
    check("Admin B no descarga fotos de A -> 404", s == 404, s)
    s, b = upload(f"{M}/adjunto", tb, {"entidad": "Equipo", "idEntidad": equipo2}, "x.png", PNG)
    check("Admin B no sube fotos a equipos de A -> 404", s == 404, (s, b))
    s, b = call("POST", f"{M}/falla", tb, {"idEquipo": equipo2, "tipo": "Fuga", "prioridad": "Alta"})
    check("Admin B no reporta fallas de equipos de A -> 404", s == 404, (s, b))
    s, b = call("GET", f"{M}/falla", tb)
    check("Admin B no ve fallas de A", s == 200 and all(f["idComp"] == compB for f in b), (s, b))
    s, b = call("POST", f"{U}/auth/me/password", tb, {"actual": "Clave#Segura1", "nueva": "Clave#Nueva22"})
    check("cambio de contraseña propia", s == 200, (s, b))
    s, b = call("POST", f"{U}/auth", body={"usuario": f"adminb{sfx}", "contrasenia": "Clave#Nueva22"})
    check("login con la nueva contraseña", s == 200, (s, b))

    print("== Errores")
    s, b = call("GET", f"{M}/planta/no-existe", root)
    check("id inexistente -> 404 ProblemDetails", s == 404 and isinstance(b, dict) and "title" in b, (s, b))
    s, b = call("POST", f"{M}/area", root, {"idPlanta": planta, "nombre": "", "tipoArea": prod})
    check("campo obligatorio vacío -> 400", s == 400, (s, b))
    s, b = call("DELETE", f"{M}/seccionarea/{secProd}", root)
    check("eliminar sección (lógico)", s == 200, (s, b))
    s, b = call("GET", f"{M}/seccionarea/{secProd}", root)
    check("sección eliminada ya no aparece -> 404", s == 404, (s, b))

    print()
    if FALLOS:
        print(f"{len(FALLOS)} verificación(es) fallaron:")
        for f in FALLOS:
            print("  - " + f)
        sys.exit(1)
    print("Todas las verificaciones pasaron.")


if __name__ == "__main__":
    main()
