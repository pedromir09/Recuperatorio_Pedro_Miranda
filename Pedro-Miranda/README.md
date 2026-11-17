# Programación II · IDRA · Recuperatorio del 2.º parcial

**Lavadero La Perla — pedidos de lavandería**
WinForms · .NET 5 · MVC · JSON · Patrones de diseño

> Mismo esqueleto que el simulacro y el parcial: mismas carpetas, mismos 8 TODOs, en el mismo orden. Cambia el negocio y cambian las reglas: **leelas bien**, no son las del taller.

---

## Antes de arrancar (5 minutos)

1. Copiá la carpeta del esqueleto al **disco ThawSpace** y renombrá la carpeta raíz a `Apellido_Nombre` (solo la carpeta de afuera, **no** el proyecto).
2. Abrí `Parcial2.sln` con Visual Studio y ejecutá con **F5**. **El esqueleto compila y abre.** Lo que todavía no está hecho muestra un mensaje del tipo *"Falta el TODO 4…"*.
3. Para ver todos los `TODO` del código: **Ctrl+Shift+F**, buscá `TODO` en **Toda la solución**.

**Duración: 2 horas.** 100 puntos, todos prácticos · Bonus: hasta 10 puntos (tope 100).

## Reglas

- Individual. **Sin IA** y sin comunicarte con compañeros.
- Podés usar el material de la cursada (presentaciones, Faro Envíos, el simulacro, tus guías).
- Si algo no te sale, **no lo borres**: dejá un comentario con qué intentaste. Un intento bien explicado suma puntos parciales.
- Cada `TODO` se corrige por separado: si te trabás en uno, pasá al siguiente.

---

## El caso

El Lavadero La Perla anota los pedidos en un cuaderno. Te piden terminar el sistema: cargar el pedido con las prendas de la lista de precios, calcular el ajuste según la forma de pago, seguir cada pedido por los lavarropas, avisarle al cliente por WhatsApp y emitir comprobantes.

Recorrido de un pedido: `EnEspera → Lavando → ParaRetirar → Entregado`. **Se puede cancelar mientras está `EnEspera` o `Lavando`**: una vez que la ropa está lista, no.

```
Parcial2/
├── Program.cs              Composition Root: el ÚNICO lugar donde se arma el sistema
├── Models/                 Pedido, ItemPedido, Prenda, EstadoPedido, IEntidad
├── Repository/             IRepository<T> + JsonRepository<T>   (persistencia)
├── Services/               Bitacora                              (Singleton)
├── Pagos/                  IPagoStrategy + estrategias + factory (Strategy + Factory)
├── Observer/               Interfaces y suscriptores              (Observer)
├── Comprobantes/           Generadores y formatos                 (Factory Method)
├── Controllers/            PedidoController, ComprobanteController
├── Views/                  Formularios (ya diseñados: no hace falta tocar el Diseñador)
└── Datos/                  prendas.json (lista de precios) y pedidos.json (pedidos de ejemplo)
```

**Lo que ya funciona:** la estructura MVC, el diseño de los formularios, la lectura del JSON (robusta: archivo inexistente, vacío o dañado), `BuscarPorId`, la lista de precios, la estrategia `PagoEfectivo`, el comprobante TXT y el algoritmo del comprobante (`GeneradorComprobante`).

**Regla de oro:** el formulario no calcula ni guarda; le pide al controller. El controller no muestra mensajes; lanza excepciones con un texto claro. La vista decide cómo mostrarlas.

---

## Consignas (100 puntos)

Hacelas en orden: cada una se apoya en la anterior.

### TODO 1 · Persistencia — 8 pts · `Repository/JsonRepository.cs`

- `Agregar(item)`: el `Id` lo asigna el repositorio: el **máximo** `Id` que haya **+ 1** (si no hay ninguno, `1`). Agregalo a la lista y reescribí el archivo (usá el método privado `Persistir`).
- `Actualizar(item)`: reemplaza el elemento con el mismo `Id` y reescribe el archivo. Si no existe → `InvalidOperationException("No existe el registro #…")`.

### TODO 2 · Modelo y validaciones — 14 pts · `Models/Pedido.cs`

| Método | Regla |
|---|---|
| `AgregarItem(prenda, cantidad)` | Si el pedido **no** está `EnEspera` → `InvalidOperationException`. Prenda `null` → `ArgumentException`. Cantidad fuera de 1…`MaximoPorItem` (20) → `ArgumentException`. Si la prenda **ya está** en el pedido, se **suma** la cantidad a esa línea (la suma tampoco puede pasar de 20). Si no, se agrega un `ItemPedido` con el **nombre y el precio actuales** de la prenda. |
| `CalcularSubtotal()` | Suma de los `Subtotal` de los ítems. |
| `AvanzarEstado()` | `EnEspera → Lavando → ParaRetirar → Entregado`. Si está finalizado (Entregado o Cancelado) → `InvalidOperationException`. |
| `Cancelar()` | Solo si está `EnEspera` **o** `Lavando`. Si no → `InvalidOperationException`. |

### TODO 3 · Singleton — 8 pts · `Services/Bitacora.cs`

Abrí **Bitácora** en la app: aparece vacía y en `bitacora.log` todas las líneas dicen `#001`. Convertí `Bitacora` en un Singleton de verdad. Todos siguen usando `Bitacora.Instancia.Registrar(...)`. Cuando el constructor quede privado, el compilador te va a marcar **un** lugar que hacía `new Bitacora()`: corregilo para que use `Bitacora.Instancia`.

### TODO 4 · Strategy — 14 pts · carpeta `Pagos/`

Creá dos estrategias que implementen `IPagoStrategy` (usá `PagoEfectivo` de modelo). `CalcularAjuste` devuelve un número **negativo si es descuento y positivo si es recargo**.

| Clase | `Nombre` | `PermiteCuotas` | `RequiereCuit` | `CalcularAjuste(subtotal, cuotas)` |
|---|---|---|---|---|
| `PagoTarjeta` | `"Tarjeta"` | `true` | `false` | 1 cuota: sin recargo · 2 cuotas: +5 % · 3 cuotas: +10 %. Cualquier otra cantidad → `ArgumentException("Con tarjeta se paga en 1, 2 o 3 cuotas.")`. |
| `PagoCuentaEmpresa` | `"Cuenta empresa"` | `false` | `true` | **Descuento** del 5 % del subtotal; si el subtotal es de **$ 100.000 o más**, el descuento es del 8 %. Subtotal **menor** a $ 20.000 → `InvalidOperationException("La cuenta empresa admite pedidos desde $ 20.000.")`. |

### TODO 5 · Factory — 8 pts · `Pagos/PagoFactory.cs`

`Crear(tipo)` recibe el texto que llega del ComboBox o del JSON (`"Efectivo"`, `"Tarjeta"`, `"Cuenta empresa"`) y devuelve la estrategia correcta. Texto desconocido → `ArgumentException`. Debe ser el **único** lugar del sistema que hace `new` de las estrategias concretas.

### TODO 6 · Controller + vista — 18 pts · `Controllers/PedidoController.cs` y `Views/FormNuevoPedido.cs`

**`CotizarPago(tipo, subtotal, cuotas)`**: factory → estrategia → cálculo, redondeado a 0 decimales.

**`ConfirmarPedido(borrador, cliente, telefono, observaciones, cuit, cuotas, tipo)`**:
1. El cliente es obligatorio y el pedido tiene que tener al menos una prenda → `ArgumentException`.
2. **Teléfono:** se le sacan los espacios y los guiones; tiene que quedar con **10 dígitos**, solo números (`223 555-0101` → `2235550101`). Si no → `ArgumentException`.
3. **Observaciones:** opcionales; sin espacios de más; si tienen más de 100 caracteres → `ArgumentException`.
4. Si la estrategia `RequiereCuit` y el CUIT está vacío → `ArgumentException`. **Preguntale a la estrategia: no compares el texto del tipo.**
5. Completá el borrador: datos sin espacios de más (el teléfono, ya limpio); `Cuit = ""` si la forma de pago no lo requiere; `Cuotas = 1` si no permite cuotas; `Ajuste` con la estrategia (redondeado) y `Total = subtotal + ajuste`; estado `EnEspera`, fecha actual.
6. Guardalo con el repositorio, **notificá el alta** (lo completás en el TODO 7) y devolvelo.

**En `FormNuevoPedido`:**
- `ActualizarControlesPago()`: al cambiar la forma de pago, `nudCuotas` se habilita solo si la estrategia `PermiteCuotas` (si no, vuelve a 1) y `txtCuit` solo si `RequiereCuit` (si no, se vacía). Usá `_controller.InfoPago(...)`: **no compares el texto del ComboBox**.
- `btnConfirmar_Click`: llamá a `ConfirmarPedido`. Si sale bien → `PedidoCreado`, `DialogResult.OK` y cerrar. Si lanza `ArgumentException` o `InvalidOperationException` → `MessageBox` con el mensaje y el formulario queda abierto.

### TODO 7 · Observer — 20 pts

1. **`PedidoController` como notificador:** lista de `IPedidoObserver`, `Suscribir` (sin duplicados), `Desuscribir` y `Notificar`. Notificar tiene que funcionar aunque un suscriptor se dé de baja **mientras** se le avisa.
2. **`HistorialObserver`**: registra `evento.Descripcion()` en la `Bitacora`.
3. **`AvisoClienteObserver`** (recibe por constructor la ruta del archivo): agrega una línea en `avisos_clientes.txt` con el formato
   `[dd/MM HH:mm] WhatsApp a {Cliente} ({Telefono}): {texto}`
   — en el **alta**: `Hola {Cliente}, recibimos tu pedido #{Id}. Total: {Total:C0}.`
   — cuando el pedido pasa a **ParaRetirar**: `Tu pedido #{Id} ya está listo para retirar. Total: {Total:C0}.`
   — en la **cancelación**: `Tu pedido #{Id} fue cancelado. Podés pasar a buscar tus prendas.`
   — en cualquier otro caso no escribe nada.
4. **`Program.cs`**: suscribí los dos observadores y el `FormPrincipal`.
5. **`FormLavadero`**: que se suscriba al abrirse y se **desuscriba al cerrarse**. Probá abrir **dos** pantallas de lavado, cerrar una y seguir avanzando pedidos: no tiene que romperse nada.

### TODO 8 · Factory Method — 10 pts · carpeta `Comprobantes/`

Agregá la **"Etiqueta para la bolsa"** (extensión `txt`) **sin tocar** `GeneradorComprobante`. Es un TXT cortito que se abrocha en la bolsa de ropa:

- `FormatoEtiqueta : IFormatoComprobante` — igual que `FormatoTxt` (un `StringBuilder` y `AppendLine`), pero con menos líneas:
  - `Encabezado`: `LAVADERO LA PERLA` y `Pedido #{Id} - {Cliente}`
  - `Item`: `{Cantidad} x {Descripcion}`
  - `Totales`: `Total: {total:C0}`
  - `Resultado`: devuelve el texto armado.
- `GeneradorComprobanteEtiqueta : GeneradorComprobante` — `Descripcion` `"Etiqueta para la bolsa"`, `Extension` `"txt"`, y su método fábrica devuelve un `FormatoEtiqueta`.
- Una línea en `ComprobanteController.GeneradoresDisponibles()`.

Ejemplo (pedido 3 de los datos de ejemplo):

```
LAVADERO LA PERLA
Pedido #3 - Hostería Los Tilos
12 x Juego de sábanas
4 x Frazada
Total: $ 115.920
```

---

## Bonus (hasta 10 pts, tope 100)

- **B1 · Transferencia (5 pts):** nueva forma de pago `"Transferencia"`: 3 % de descuento, sin cuotas, sin CUIT. Sin modificar ninguna estrategia existente ni el controller.
- **B2 · Cancelación en lavado (5 pts):** un observador nuevo que, cuando se cancela un pedido que **estaba `Lavando`** (mirá `EstadoAnterior` del evento), registra en la bitácora `ATENCIÓN encargada: se canceló el pedido #{Id} que ya estaba en lavado ({Cliente})`. Una clase nueva + una línea en `Program.cs`.

---

## Para probar

| Caso | Resultado esperado |
|---|---|
| 2 acolchados 2 plazas + 3 camisas, tarjeta en 3 cuotas | Subtotal $ 49.500 · Recargo $ 4.950 · Total $ 54.450 |
| El mismo pedido en efectivo | Descuento −$ 4.950 · Total $ 44.550 |
| El mismo pedido, tarjeta en 2 cuotas | Recargo $ 2.475 · Total $ 51.975 |
| 10 manteles + 5 juegos de sábanas, cuenta empresa | Subtotal $ 102.500 · Descuento −$ 8.200 · Total $ 94.300 |
| 2 pantalones + 2 juegos de sábanas, cuenta empresa | Subtotal $ 20.000 justo: pasa · Descuento −$ 1.000 |
| 1 camisa, cuenta empresa | Muestra el mínimo de $ 20.000 |
| Tarjeta en 4 cuotas | El formulario muestra "Con tarjeta se paga en 1, 2 o 3 cuotas." |
| Teléfono `223 555-0101` | Se guarda como `2235550101` |
| Teléfono `555-0101` | Mensaje: el teléfono tiene que tener 10 números |
| 15 camisas y después 6 más | Mensaje: no se pueden cargar más de 20 |
| Cancelar el pedido 3 (ParaRetirar) | Mensaje: solo se cancela un pedido EnEspera o Lavando |
| Cancelar el pedido 4 (Lavando) | Se cancela |
| Facturado con los pedidos de ejemplo | $ 290.120 (no cuenta el cancelado) |
| Abrir Bitácora después de usar la app | Muestra todo lo registrado, numerado #001, #002, #003… |

---

## Entrega

1. Respondé las tres preguntas de `CIERRE.md`.
2. Cerrá Visual Studio. Borrá las carpetas `bin` y `obj`.
3. Comprimí la carpeta `Apellido_Nombre` en un **ZIP** con el mismo nombre y entregalo por el medio que indique el docente.

**El código tiene que compilar.** Si algo no compila, comentalo y explicá qué querías hacer.
