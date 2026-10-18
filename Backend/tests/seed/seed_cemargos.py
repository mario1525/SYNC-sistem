"""
Datos de prueba para la compañía CEMARGOS: 3 plantas de concreto con todas sus etapas (áreas),
secciones y maquinaria, 4 cuadrillas por planta, especialidades y usuarios de todos los roles
con credenciales.

Uso:
  python seed_cemargos.py --api http://localhost:7021/apigateway --root-user root --root-pass '...'
                          [--comp "CEMARGOS"] [--password 'Cemargos#2026']

Idempotente: lo que ya existe (por nombre / usuario) se reutiliza, no se duplica.
Escribe las credenciales creadas en cemargos_usuarios.csv (junto a este script, fuera de git).
"""
import argparse
import csv
import json
import os
import sys
import urllib.error
import urllib.parse
import urllib.request

API = ""
TOKEN = ""


def call(method, path, body=None, ok=(200, 201)):
    data = None if body is None else json.dumps(body).encode()
    req = urllib.request.Request(API + path, data=data, method=method)
    req.add_header("Content-Type", "application/json")
    if TOKEN:
        req.add_header("Authorization", f"Bearer {TOKEN}")
    try:
        with urllib.request.urlopen(req, timeout=30) as r:
            raw = r.read()
            return json.loads(raw) if raw else None
    except urllib.error.HTTPError as e:
        raw = e.read().decode(errors="replace")
        if e.code in ok:
            return json.loads(raw) if raw else None
        sys.exit(f"ERROR {method} {path} -> {e.code}: {raw}\nbody={body}")


def q(**kw):
    return "?" + urllib.parse.urlencode({k: v for k, v in kw.items() if v is not None})


def get_or_create(listar, crear, body, clave="nombre"):
    """Busca en la lista un elemento con el mismo valor de `clave`; si no existe lo crea."""
    for x in call("GET", listar) or []:
        if (x.get(clave) or "").strip().lower() == str(body[clave]).strip().lower():
            return x["id"], False
    return call("POST", crear, body)["id"], True


# ------------------------------------------------------------------ catálogo de la planta tipo
PRODUCCION, BODEGA, PATIO = "Produccion", "Bodega", "Patio"

# Etapa (área) -> tipo de área, secciones y equipos por sección:
# (nombre, marca, fabricante, modelo, potencia kW, capacidad, peso kg, función)
ETAPAS = [
    ("01 Recepción y acopio de agregados", PATIO, {
        "Patio de arena": [
            ("Cargador frontal", "Caterpillar", "Caterpillar Inc.", "950 GC", 168, 4, 18500, "Alimentar tolvas de agregados"),
        ],
        "Patio de grava": [
            ("Cargador frontal", "Komatsu", "Komatsu Ltd.", "WA380-8", 142, 3, 17000, "Alimentar tolvas de agregados"),
        ],
        "Tolvas de agregados": [
            ("Tolva de agregados 4 compartimientos", "Liebherr", "Liebherr Mischtechnik", "Betomix 4x30", 0, 120, 9500, "Almacenar arena y gravas para dosificar"),
            ("Vibrador de tolva", "Martin Engineering", "Martin Engineering", "BRUTE 2", 1, 0, 25, "Evitar atascamiento de agregado"),
        ],
    }),
    ("02 Almacenamiento de cemento", PRODUCCION, {
        "Silos de cemento": [
            ("Silo de cemento 1", "WAM", "WAM Group", "SILO-100T", 0, 100, 12000, "Almacenar cemento a granel"),
            ("Silo de cemento 2", "WAM", "WAM Group", "SILO-100T", 0, 100, 12000, "Almacenar cemento a granel"),
            ("Filtro colector de polvo", "WAM", "WAM Group", "SILOTOP R07", 2, 0, 350, "Captar polvo en la carga neumática del silo"),
        ],
        "Transporte de cemento": [
            ("Tornillo sinfín de cemento 1", "WAM", "WAM Group", "TU 273", 15, 80, 900, "Transportar cemento del silo a la báscula"),
            ("Tornillo sinfín de cemento 2", "WAM", "WAM Group", "TU 273", 15, 80, 900, "Transportar cemento del silo a la báscula"),
        ],
    }),
    ("03 Transporte de agregados", PRODUCCION, {
        "Banda transportadora": [
            ("Banda transportadora inclinada", "Continental", "ContiTech", "BT-800x45", 22, 250, 6500, "Llevar agregados de tolvas a la mezcladora"),
            ("Motorreductor de banda", "SEW-Eurodrive", "SEW-Eurodrive", "KA97", 22, 0, 420, "Accionar la banda transportadora"),
        ],
    }),
    ("04 Dosificación y pesaje", PRODUCCION, {
        "Básculas": [
            ("Báscula de agregados", "Mettler Toledo", "Mettler Toledo", "IND570", 1, 6000, 1500, "Pesar agregados por bachada"),
            ("Báscula de cemento", "Mettler Toledo", "Mettler Toledo", "IND570", 1, 1500, 800, "Pesar cemento por bachada"),
        ],
        "Agua y aditivos": [
            ("Dosificador de aditivos", "Sika", "Sika AG", "SikaDispenser 4", 1, 200, 150, "Dosificar aditivos químicos"),
            ("Medidor de agua", "Endress+Hauser", "Endress+Hauser", "Promag W 400", 1, 600, 40, "Medir el agua de mezcla"),
            ("Bomba de agua de mezcla", "Grundfos", "Grundfos", "CR 32-4", 11, 32, 180, "Suministrar agua a la mezcladora"),
        ],
        "Cabina de control": [
            ("Sistema de control de planta", "Command Alkon", "Command Alkon", "COMMANDbatch", 1, 0, 60, "Automatizar dosificación y despacho"),
        ],
    }),
    ("05 Mezclado", PRODUCCION, {
        "Mezcladora central": [
            ("Mezcladora doble eje", "SICOMA", "SICOMA", "MAO 4500/3000", 110, 3, 11500, "Mezclar concreto premezclado"),
            ("Unidad hidráulica de compuerta", "Rexroth", "Bosch Rexroth", "ABPAC", 4, 0, 120, "Abrir y cerrar la compuerta de descarga"),
        ],
    }),
    ("06 Despacho y carga de mixers", PRODUCCION, {
        "Zona de carga": [
            ("Tolva de carga de mixers", "Liebherr", "Liebherr Mischtechnik", "LCT-10", 0, 10, 2200, "Descargar la mezcla al camión mixer"),
            ("Báscula camionera", "Toledo", "Mettler Toledo", "VTC221", 1, 80000, 15000, "Pesaje de despacho"),
        ],
    }),
    ("07 Flota de transporte", PATIO, {
        "Patio de mixers": [
            ("Camión mixer 8 m³ #1", "Kenworth", "PACCAR", "T800 + McNeilus", 336, 8, 13500, "Transportar concreto a obra"),
            ("Camión mixer 8 m³ #2", "Kenworth", "PACCAR", "T800 + McNeilus", 336, 8, 13500, "Transportar concreto a obra"),
            ("Camión mixer 8 m³ #3", "International", "Navistar", "HV607 + McNeilus", 298, 8, 13200, "Transportar concreto a obra"),
            ("Camión mixer 8 m³ #4", "Mack", "Mack Trucks", "Granite + London", 317, 8, 13800, "Transportar concreto a obra"),
        ],
        "Patio de bombas": [
            ("Autobomba de concreto pluma 36 m", "Putzmeister", "Putzmeister", "M36-5", 300, 160, 26000, "Bombear concreto en obra"),
            ("Bomba estacionaria de concreto", "Schwing", "Schwing Stetter", "SP 1800", 75, 80, 6000, "Bombear concreto por tubería"),
        ],
    }),
    ("08 Control de calidad", PRODUCCION, {
        "Laboratorio": [
            ("Prensa de compresión de cilindros", "Controls", "Controls Group", "Automax 5", 2, 2000, 900, "Ensayar resistencia del concreto"),
            ("Cámara húmeda de curado", "Humboldt", "Humboldt Mfg.", "H-3070", 3, 300, 400, "Curar probetas de concreto"),
        ],
    }),
    ("09 Tratamiento de agua y reciclaje", PRODUCCION, {
        "Lavado de mixers": [
            ("Recicladora de concreto", "Liebherr", "Liebherr Mischtechnik", "LRS 806", 15, 20, 5200, "Recuperar agregado del concreto devuelto"),
            ("Bomba sumergible de lodos", "Flygt", "Xylem", "2640", 7, 25, 90, "Evacuar lodos de las piscinas"),
        ],
        "Piscinas de sedimentación": [
            ("Agitador de piscina", "Flygt", "Xylem", "SR 4650", 4, 0, 150, "Mantener lodos en suspensión"),
        ],
    }),
    ("10 Servicios industriales", PRODUCCION, {
        "Cuarto de compresores": [
            ("Compresor de tornillo", "Atlas Copco", "Atlas Copco", "GA 37 VSD", 37, 6, 900, "Aire comprimido para válvulas y filtros"),
            ("Secador de aire", "Atlas Copco", "Atlas Copco", "FD 120", 2, 120, 160, "Secar aire comprimido"),
        ],
        "Subestación eléctrica": [
            ("Planta eléctrica de respaldo", "Cummins", "Cummins Inc.", "C500 D6", 400, 500, 4500, "Energía de respaldo"),
            ("Transformador 500 kVA", "Siemens", "Siemens", "GEAFOL 500", 0, 500, 1800, "Alimentación de media a baja tensión"),
        ],
    }),
    ("11 Bodega de repuestos", BODEGA, {
        "Estantería de repuestos": [
            ("Motorreductor de repuesto", "SEW-Eurodrive", "SEW-Eurodrive", "KA97", 22, 0, 420, "Repuesto de banda transportadora"),
        ],
        "Zona de lubricantes": [],
    }),
]

PLANTAS = [
    ("Planta Concreto Barranquilla", "Caribe - Atlántico", "BAQ", "Calle 30 vía al aeropuerto"),
    ("Planta Concreto Cartagena", "Caribe - Bolívar", "CTG", "Zona industrial Mamonal"),
    ("Planta Concreto Santa Marta", "Caribe - Magdalena", "SMR", "Troncal del Caribe km 3"),
]

# Cuadrillas por planta: (sufijo, especialidad)
CUADRILLAS = [
    ("Mecánica", "Mecánica"),
    ("Eléctrica e Instrumentación", "Eléctrica"),
    ("Lubricación y Predictivo", "Lubricación"),
    ("Flota y Equipo Móvil", "Mecánica automotriz"),
]

ESPECIALIDADES = [
    ("Mecánica", "Mantenimiento mecánico de equipos de planta"),
    ("Eléctrica", "Mantenimiento eléctrico, motores, tableros e instrumentación"),
    ("Lubricación", "Lubricación, análisis de aceite y mantenimiento predictivo"),
    ("Mecánica automotriz", "Mantenimiento de camiones mixer, bombas y equipo móvil"),
    ("Hidráulica", "Sistemas hidráulicos y neumáticos"),
]

NOMBRES = ["Carlos", "Andrea", "Luis", "María", "Jorge", "Paola", "Andrés", "Diana", "Javier", "Natalia",
           "Ricardo", "Camila", "Fernando", "Laura", "Héctor", "Valentina", "Óscar", "Juliana", "Mauricio",
           "Daniela", "Raúl", "Carolina", "Iván", "Mónica", "Edwin", "Sandra", "Wilson", "Adriana", "Fabián", "Lina"]
APELLIDOS = ["Pérez", "Gómez", "Rodríguez", "Martínez", "Herrera", "Castro", "Navarro", "Ospina", "Mendoza",
             "Rojas", "Vargas", "Torres", "Jiménez", "Morales", "Ortiz", "Suárez", "Cárdenas", "Ríos", "Pacheco",
             "Salazar", "Charris", "De la Hoz", "Barrios", "Pineda", "Polo", "Escorcia", "Fontalvo", "Mejía", "Acosta", "Rúa"]

LOTO = ("Aislamiento y bloqueo (LOTO)", [("Permiso de trabajo firmado", False), ("Candado y tarjeta instalados", True),
                                        ("Energía cero verificada", False)])
ENTREGA = ("Prueba y entrega", [("Equipo probado en vacío sin ruidos ni vibraciones anormales", False),
                                ("Área limpia y retiro de bloqueos", True)])

# Guías: nombre, especialidad, intervalo (días), personal, duración (h), equipos (prefijo del nombre),
# campos de texto y pasos [(paso, [(verificación, requiere foto)])]
GUIAS = [
    dict(nombre="PM 250 h cargador frontal", esp="Mecánica automotriz", intervalo=15, personal=2, duracion=4,
         equipos=["Cargador frontal"],
         descripcion="Mantenimiento preventivo cada 250 horas de operación del cargador frontal.",
         herramientas="Llaves combinadas, pistola de grasa, bomba de trasiego, torquímetro",
         insumos="Aceite 15W-40, filtro de aceite, filtro de combustible, grasa EP2",
         seguridadInd="Casco, gafas, guantes de nitrilo, botas punta de acero",
         seguridadAmb="Recolectar aceite usado en recipiente rotulado; kit antiderrames disponible",
         proced=[LOTO,
                 ("Cambio de aceite y filtros", [("Aceite de motor drenado y reemplazado", False),
                                                 ("Filtros de aceite y combustible cambiados", True)]),
                 ("Engrase de articulaciones", [("Pasadores de brazo y balde engrasados", False),
                                                ("Articulación central engrasada", False)]),
                 ("Inspección general", [("Llantas sin cortes y con presión correcta", False),
                                         ("Dientes y cuchilla del balde en buen estado", True),
                                         ("Sin fugas hidráulicas", False)]),
                 ENTREGA]),
    dict(nombre="Mantenimiento mezcladora doble eje", esp="Mecánica", intervalo=30, personal=3, duracion=6,
         equipos=["Mezcladora doble eje", "Unidad hidráulica de compuerta"],
         descripcion="Inspección de desgaste de paletas, brazos y blindajes; ajuste de holguras y lubricación de sellos.",
         herramientas="Galgas, llaves de impacto, martillo de bronce, linterna, hidrolavadora",
         insumos="Paletas y blindajes de repuesto, grasa para sellos, tornillería grado 8",
         seguridadInd="Arnés para espacio confinado, mascarilla para polvo, protección auditiva",
         seguridadAmb="Disponer residuos de concreto en la piscina de reciclaje",
         importante="Ingreso a la cuba solo con permiso de espacio confinado y vigía",
         proced=[LOTO,
                 ("Limpieza interna de la cuba", [("Costras de concreto removidas", True)]),
                 ("Inspección de elementos de desgaste", [("Holgura paleta-blindaje entre 3 y 5 mm", False),
                                                          ("Paletas y brazos sin fisuras", True),
                                                          ("Blindajes con espesor mayor al mínimo", False)]),
                 ("Sellos y lubricación", [("Sellos de eje sin fuga de lechada", False),
                                           ("Bomba de lubricación de sellos operando", False)]),
                 ("Compuerta de descarga", [("Compuerta abre y cierra completa", False),
                                            ("Nivel de aceite hidráulico correcto", False)]),
                 ENTREGA]),
    dict(nombre="Inspección de banda transportadora", esp="Mecánica", intervalo=15, personal=2, duracion=3,
         equipos=["Banda transportadora", "Motorreductor de banda"],
         descripcion="Inspección de banda, rodillos, raspadores y motorreductor de la banda de agregados.",
         herramientas="Llaves, termómetro infrarrojo, tacómetro, alineador de banda",
         insumos="Rodillos de repuesto, láminas de raspador, aceite ISO VG 220",
         seguridadInd="Casco, gafas, guantes, protección auditiva",
         seguridadAmb="Recoger derrames de agregado bajo la banda",
         proced=[LOTO,
                 ("Banda y empalmes", [("Banda sin cortes ni desprendimientos", True),
                                       ("Empalme en buen estado", False),
                                       ("Banda alineada y con tensión correcta", False)]),
                 ("Rodillos y raspadores", [("Rodillos giran libres, sin ruido", False),
                                            ("Raspador primario ajustado", False)]),
                 ("Motorreductor", [("Nivel de aceite del reductor correcto", False),
                                    ("Temperatura de carcasa menor a 80 °C", False)]),
                 ENTREGA]),
    dict(nombre="Calibración de básculas", esp="Eléctrica", intervalo=90, personal=2, duracion=4,
         equipos=["Báscula de agregados", "Báscula de cemento", "Báscula camionera"],
         descripcion="Verificación y calibración de básculas de dosificación y despacho con pesas patrón.",
         herramientas="Pesas patrón certificadas, multímetro, indicador de pesaje, nivel",
         insumos="Formato de certificado de calibración",
         seguridadInd="Casco, gafas, guantes",
         seguridadAmb="N/A",
         importante="Usar pesas con certificado de calibración vigente",
         proced=[("Preparación", [("Báscula vacía y limpia", False), ("Pesas patrón con certificado vigente", True)]),
                 ("Verificación de celdas de carga", [("Celdas sin golpes ni humedad", False),
                                                      ("Cableado y caja de conexión en buen estado", False)]),
                 ("Calibración", [("Cero ajustado", False), ("Error en 25/50/100 % dentro de ±0,5 %", True)]),
                 ("Registro", [("Certificado diligenciado y firmado", True)])]),
    dict(nombre="Mantenimiento de silos de cemento y colector", esp="Mecánica", intervalo=60, personal=3, duracion=5,
         equipos=["Silo de cemento", "Filtro colector de polvo"],
         descripcion="Inspección estructural del silo, válvulas de alivio, indicadores de nivel y mangas del colector.",
         herramientas="Llaves, martillo de goma, linterna, multímetro",
         insumos="Mangas filtrantes, empaques de válvula",
         seguridadInd="Arnés y línea de vida para trabajo en alturas, mascarilla N95",
         seguridadAmb="Disponer mangas usadas como residuo industrial",
         importante="Trabajo en alturas: requiere permiso y personal certificado",
         proced=[("Trabajo en alturas", [("Permiso de alturas firmado", False), ("Línea de vida instalada", True)]),
                 ("Estructura del silo", [("Sin corrosión ni fisuras en soldaduras", True),
                                          ("Escalera y barandas firmes", False)]),
                 ("Válvulas e indicadores", [("Válvula de alivio de presión libre", False),
                                             ("Indicadores de nivel alto y bajo funcionando", False)]),
                 ("Colector de polvo", [("Mangas sin roturas", True), ("Limpieza por pulso de aire operando", False)])]),
    dict(nombre="Lubricación de tornillos sinfín", esp="Lubricación", intervalo=30, personal=1, duracion=2,
         equipos=["Tornillo sinfín de cemento"],
         descripcion="Lubricación de rodamientos, revisión del reductor y de los apoyos intermedios del sinfín.",
         herramientas="Pistola de grasa, termómetro infrarrojo, analizador de vibraciones",
         insumos="Grasa EP2, aceite ISO VG 220",
         seguridadInd="Casco, gafas, guantes, mascarilla para polvo",
         seguridadAmb="Recolectar grasa usada",
         proced=[LOTO,
                 ("Lubricación", [("Rodamientos de cabeza y cola engrasados", False),
                                  ("Apoyos intermedios engrasados", False)]),
                 ("Reductor", [("Nivel de aceite correcto", False), ("Sin fugas por retenedores", False)]),
                 ("Predictivo", [("Vibración en rango aceptable", False), ("Lectura de vibración registrada", True)])]),
    dict(nombre="PM camión mixer", esp="Mecánica automotriz", intervalo=30, personal=2, duracion=6,
         equipos=["Camión mixer"],
         descripcion="Mantenimiento preventivo del camión mixer: motor, frenos, sistema hidráulico y tambor.",
         herramientas="Scanner de diagnóstico, llaves, torquímetro, manómetro",
         insumos="Aceite 15W-40, filtros, aceite hidráulico ISO 46, grasa",
         seguridadInd="Casco, gafas, guantes, botas, chaleco reflectivo",
         seguridadAmb="Lavado del tambor solo en la zona de reciclaje",
         proced=[("Motor", [("Aceite y filtros cambiados", True), ("Nivel de refrigerante correcto", False),
                            ("Correas sin grietas", False)]),
                 ("Frenos y llantas", [("Pastillas/bandas con espesor mínimo", False),
                                       ("Llantas sin cortes y con presión correcta", True)]),
                 ("Sistema hidráulico del tambor", [("Bomba y motor hidráulico sin fugas", False),
                                                    ("Tambor gira en ambos sentidos", False)]),
                 ("Tambor", [("Aletas internas sin desgaste excesivo", True), ("Rodillos del tambor engrasados", False)]),
                 ENTREGA]),
    dict(nombre="Mantenimiento de bomba de concreto", esp="Hidráulica", intervalo=30, personal=2, duracion=5,
         equipos=["Autobomba de concreto", "Bomba estacionaria de concreto"],
         descripcion="Revisión de pistones, válvula S, anillo de corte, tubería y sistema hidráulico de la bomba.",
         herramientas="Manómetro hidráulico, calibrador de espesores, llaves de impacto",
         insumos="Pistones de bombeo, anillo de corte, placa de desgaste, aceite hidráulico ISO 46",
         seguridadInd="Casco, gafas, guantes, protección auditiva",
         seguridadAmb="Disponer concreto residual en la piscina de reciclaje",
         importante="Despresurizar acumuladores antes de intervenir",
         proced=[LOTO,
                 ("Sistema de bombeo", [("Pistones sin desgaste ni paso de lechada", True),
                                        ("Holgura anillo de corte - placa dentro de tolerancia", False)]),
                 ("Tubería y pluma", [("Espesor de tubería mayor al mínimo", False),
                                      ("Abrazaderas y empaques en buen estado", False)]),
                 ("Hidráulico", [("Presión de trabajo según fabricante", False), ("Sin fugas en mangueras", True)]),
                 ENTREGA]),
    dict(nombre="Mantenimiento compresor de tornillo y secador", esp="Lubricación", intervalo=60, personal=1, duracion=3,
         equipos=["Compresor de tornillo", "Secador de aire"],
         descripcion="Cambio de filtros y aceite del compresor, revisión de purgas y punto de rocío del secador.",
         herramientas="Llaves, manómetro, termómetro",
         insumos="Aceite para compresor, filtro de aire, filtro de aceite, separador",
         seguridadInd="Gafas, guantes, protección auditiva",
         seguridadAmb="Recolectar condensados con aceite para tratamiento",
         proced=[LOTO,
                 ("Compresor", [("Filtro de aire cambiado", False), ("Aceite y filtro de aceite cambiados", True),
                                ("Sin fugas de aire en conexiones", False)]),
                 ("Secador", [("Purga automática funcionando", False), ("Punto de rocío dentro del rango", False)]),
                 ENTREGA]),
    dict(nombre="Mantenimiento planta eléctrica y transformador", esp="Eléctrica", intervalo=90, personal=2, duracion=4,
         equipos=["Planta eléctrica de respaldo", "Transformador 500 kVA"],
         descripcion="Prueba con carga de la planta de respaldo, revisión de baterías y termografía del transformador.",
         herramientas="Multímetro, pinza amperimétrica, cámara termográfica, densímetro",
         insumos="Aceite 15W-40, filtros, refrigerante, electrolito",
         seguridadInd="Ropa ignífuga, guantes dieléctricos, careta, botas dieléctricas",
         seguridadAmb="Recolectar aceite y baterías usadas como residuo peligroso",
         importante="Coordinar con producción la transferencia de carga",
         proced=[("Planta eléctrica", [("Nivel de aceite y refrigerante correctos", False),
                                       ("Baterías con carga mayor a 12,4 V", False),
                                       ("Arranque y transferencia automática exitosos", True)]),
                 ("Transformador", [("Termografía sin puntos calientes", True),
                                    ("Sin fugas de aceite ni ruidos anormales", False),
                                    ("Puesta a tierra verificada", False)]),
                 ("Registro", [("Horómetro y lecturas registrados", False)])]),
]


def main():
    global API, TOKEN
    ap = argparse.ArgumentParser()
    ap.add_argument("--api", default="http://localhost:7021/apigateway")
    ap.add_argument("--root-user", required=True)
    ap.add_argument("--root-pass", required=True)
    ap.add_argument("--comp", default="CEMARGOS")
    ap.add_argument("--password", default="Cemargos#2026", help="contraseña de prueba para todos los usuarios")
    a = ap.parse_args()
    API = a.api.rstrip("/")

    TOKEN = call("POST", "/auth", {"usuario": a.root_user, "contrasenia": a.root_pass})["token"]
    comp = next((c for c in call("GET", "/comp") if a.comp.lower() in c["nombre"].lower()), None)
    if not comp:
        sys.exit(f"No se encontró la compañía '{a.comp}'.")
    idComp = comp["id"]
    print(f"Compañía: {comp['nombre']} ({idComp})")

    tipos = {t["nombre"]: t["id"] for t in call("GET", "/tipoarea")}

    # ------------------------------------------------------------ especialidades
    esp = {}
    for nombre, desc in ESPECIALIDADES:
        esp[nombre], _ = get_or_create(f"/esp{q(IdComp=idComp)}", "/esp",
                                       {"nombre": nombre, "descripcion": desc, "idComp": idComp})
    print(f"Especialidades: {len(esp)}")

    # ------------------------------------------------------------ plantas, etapas, secciones, equipos
    existentes_eq = {e["nSerie"] for e in call("GET", f"/equipo{q(IdComp=idComp)}") if e.get("nSerie")}
    cuads = {}  # (planta, sufijo) -> id
    tot = {"areas": 0, "secciones": 0, "equipos": 0}
    for nombre, region, cod, _dir in PLANTAS:
        idPlanta, nueva = get_or_create(f"/planta{q(IdComp=idComp)}", "/planta",
                                        {"nombre": nombre, "region": region, "idComp": idComp})
        print(f"\n{'+' if nueva else '='} {nombre}")
        n = 0
        for etapa, tipo, secciones in ETAPAS:
            idArea, _ = get_or_create(f"/area{q(IdPlanta=idPlanta)}", "/area",
                                      {"idPlanta": idPlanta, "nombre": etapa, "tipoArea": tipos[tipo]})
            tot["areas"] += 1
            for seccion, equipos in secciones.items():
                idSec, _ = get_or_create(f"/seccionarea{q(IdArea=idArea)}", "/seccionarea",
                                         {"idArea": idArea, "nombre": seccion})
                tot["secciones"] += 1
                ubic = "Almacen" if tipo == BODEGA else "Produccion"
                for (eqn, marca, fab, modelo, pot, cap, peso, func) in equipos:
                    n += 1
                    serie = f"CMG-{cod}-{n:03d}"
                    tot["equipos"] += 1
                    if serie in existentes_eq:
                        continue
                    call("POST", "/equipo", {
                        "nombre": f"{eqn} {cod}", "descripcion": f"{eqn} - {nombre}", "idComp": idComp,
                        "modelo": modelo, "nSerie": serie, "fabricante": fab, "marca": marca, "funcion": func,
                        "potencia": pot, "capacidad": cap, "peso": peso, "anioFabricacion": 2016 + n % 9,
                        "caracteristicas": f"Etapa: {etapa}. Sección: {seccion}.",
                        "ubicacion": {"tipoUbicacion": ubic, "idSeccionArea": idSec}})
        print(f"  {len(ETAPAS)} etapas, {n} equipos")

        for sufijo, _e in CUADRILLAS:
            cuads[(cod, sufijo)], _ = get_or_create(f"/cuad{q(IdComp=idComp)}", "/cuad",
                                                    {"nombre": f"{cod} - {sufijo}", "idComp": idComp})
    print(f"\nCuadrillas: {len(cuads)}")

    # ------------------------------------------------------------ usuarios
    usuarios = call("GET", f"/users{q(IdComp=idComp)}") or []
    por_ident = {u["identificacion"]: u for u in usuarios}
    filas = []
    i = 0

    def persona(rol, cargo, usuario, idCuad=None, idEsp=None):
        nonlocal i
        ident = 1_100_000_000 + int(idComp.replace("-", "")[:6], 16) % 1000 * 1000 + i
        nom, ape = NOMBRES[i % len(NOMBRES)], APELLIDOS[(i * 7) % len(APELLIDOS)]
        i += 1
        u = por_ident.get(ident)
        if u:
            idUser = u["id"]
        else:
            idUser = call("POST", "/users", {
                "nombre": nom, "apellido": ape, "identificacion": ident, "correo": f"{usuario}@cemargos.test",
                "idComp": idComp, "idCuad": idCuad, "idEsp": idEsp, "cargo": cargo, "rol": rol})["id"]
        creds = call("GET", f"/credential{q(IdUser=idUser)}") or []
        if not creds:
            call("POST", "/credential", {"usuario": usuario, "contrasenia": a.password, "idUser": idUser})
        filas.append([rol, usuario, a.password, f"{nom} {ape}", cargo, idCuad and next(
            f"{k[0]} - {k[1]}" for k, v in cuads.items() if v == idCuad) or ""])

    persona("Root", "Superusuario de pruebas", "root.cemargos")
    persona("Admin", "Gerente de mantenimiento", "admin.cemargos")
    persona("Admin", "Coordinador de confiabilidad", "admin2.cemargos")
    for _n, _r, cod, _d in PLANTAS:
        c = cod.lower()
        persona("Supervisor", f"Jefe de mantenimiento {cod}", f"sup.{c}", cuads[(cod, CUADRILLAS[0][0])], esp["Mecánica"])
        persona("Supervisor", f"Supervisor eléctrico {cod}", f"sup2.{c}", cuads[(cod, CUADRILLAS[1][0])], esp["Eléctrica"])
        for k, (sufijo, especialidad) in enumerate(CUADRILLAS, start=1):
            for j in (1, 2):
                persona("Tecnico", f"Técnico {especialidad.lower()}", f"tec.{c}.c{k}.{j}",
                        cuads[(cod, sufijo)], esp[especialidad])

    # ------------------------------------------------------------ guías y asignación a equipos
    equipos = call("GET", f"/equipo{q(IdComp=idComp)}") or []
    guias = {g["nombre"].lower(): g["id"] for g in call("GET", f"/guia{q(IdComp=idComp)}") or []}
    asignaciones = 0
    for g in GUIAS:
        idGuia = guias.get(g["nombre"].lower())
        if not idGuia:
            body = {k: v for k, v in g.items() if k not in ("esp", "equipos", "proced")}
            body.update(idComp=idComp, idEsp=esp[g["esp"]], proced=[
                {"nombre": paso, "valid": [{"nombre": v, "requiereFoto": foto} for v, foto in valids]}
                for paso, valids in g["proced"]])
            idGuia = call("POST", "/guia", body)["id"]
        for e in equipos:
            if any(e["nombre"].startswith(p) for p in g["equipos"]):
                call("POST", "/guiaequipo", {"idGuia": idGuia, "idEquipo": e["id"], "intervalo": g["intervalo"]},
                     ok=(200, 201, 409))
                asignaciones += 1
    print(f"Guías: {len(GUIAS)}, asignadas a {asignaciones} equipos")

    salida = os.path.join(os.path.dirname(os.path.abspath(__file__)), "cemargos_usuarios.csv")
    with open(salida, "w", newline="", encoding="utf-8-sig") as f:
        w = csv.writer(f)
        w.writerow(["Rol", "Usuario", "Contraseña", "Nombre", "Cargo", "Cuadrilla"])
        w.writerows(filas)
    print(f"Usuarios: {len(filas)} -> {salida}")
    print(f"Totales: 3 plantas, {tot['areas']} etapas, {tot['secciones']} secciones, {tot['equipos']} equipos")


if __name__ == "__main__":
    main()
