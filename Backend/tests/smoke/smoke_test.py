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
