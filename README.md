# Suite de Cálculos Eléctricos y Electrónicos
Nota: Todas las ingenierías, además de la belleza del arte técnico, son complejas, este proyecto no pretende cubrir campos tan amplios, solo es algo que me ayudó mucho en la época del DOS y que ahora estoy reescribiendo. Inicia con un simple calculo de fuentes buck PWM y fui añadiendo usos diarios.

Aplicación de escritorio Windows Forms para diseño y análisis de convertidores Buck DC-DC con editor de esquemáticos integrado. Suite completa de herramientas para electrónica de potencia con motor de cálculo avanzado y capacidades de diseño visual.

## Índice

- [Características Principales](#características-principales)
- [Requisitos del Sistema](#requisitos-del-sistema)
- [Arquitectura del Proyecto](#arquitectura-del-proyecto)
- [Motor de Cálculo](#motor-de-cálculo)
- [Editor de Esquemáticos](#editor-de-esquemáticos)
- [Instalación](#instalación)
- [Uso](#uso)
- [Modelos de Datos](#modelos-de-datos)
- [Fórmulas Implementadas](#fórmulas-implementadas)
- [Módulos Adicionales](#módulos-adicionales)
- [Desarrollo](#desarrollo)
- [Roadmap](#roadmap)

---

## Características Principales

### Motor de Cálculo Buck Converter
- **Análisis Fundamental**: Ciclo de trabajo, potencia entrada/salida, eficiencia estimada y real
- **Diseño de Inductor**: Cálculo de inductancia mínima, ripple de corriente, corriente pico y RMS
- **Diseño de Capacitor**: Dimensionado de capacitancia de salida y cálculo de ESR máximo
- **Valores Comerciales**: Ajuste automático a series E12/E24 para inductancias y capacitancias
- **Configuración PWM**: Cálculo de componentes para controlador UC3843 (Rt/Ct)
- **Análisis de Pérdidas**: MOSFET (conducción/switching), diodo, núcleo del inductor
- **Análisis Térmico**: Estimación de temperaturas de juntura (MOSFET, diodo, inductor)

### Editor de Esquemáticos WYSIWYG
- **Canvas Interactivo**: Zoom, pan, grid con snap-to-grid
- **Librería de Componentes**: Resistores, capacitores, inductores, terminales, ICs, labels
- **Toolbox Acoplable**: Drag & drop para inserción rápida de componentes
- **Sistema de Cableado Avanzado**: 
  - Modos de ruteo: Direct, Orthogonal (Manhattan), Auto
  - Waypoints editables, vista previa en tiempo real
  - Conexión automática a pines
- **Edición en Tiempo Real**: PropertyGrid para modificar propiedades de componentes
- **Operaciones**: Copiar/pegar, duplicar, eliminar, rotar 90°, espejar, alinear, distribuir
- **Persistencia**: Guardado/carga en JSON, exportación a PNG

### Herramientas Adicionales
- **Cálculo de Protección de Motores**: Dimensionado de protecciones térmicas
- **Cálculo de Reactores VFD**: Diseño de reactores para variadores de frecuencia
- **Diseño de Transformadores**: Cálculo de transformadores de potencia
- **Análisis VFD**: Herramientas para variadores de frecuencia

---

## Requisitos del Sistema

### Software
- **.NET 10 SDK** (o superior)
- **Visual Studio 2026** (recomendado) o cualquier IDE compatible con C# 14.0
- **Windows 10/11** (64-bit)

### Hardware Recomendado
- Procesador x64 de 2 GHz o superior
- 4 GB RAM mínimo (8 GB recomendado)
- 500 MB de espacio en disco
- Pantalla con resolución mínima 1366x768

---

## Arquitectura del Proyecto

```
BuckConverterCalculator/
│
├── BuckConverterCalculator.csproj    # Archivo de proyecto
├── MainForm.cs                       # Formulario principal de la aplicación
│
├── Core/
│   ├── BuckCalculator.cs             # Motor de cálculo del convertidor Buck
│   ├── DataModels.cs                 # Modelos: DesignParameters, CalculationResults
│   └── CommercialValues.cs           # Series E12/E24 y ajuste de valores
│
├── Forms/
│   ├── CalculoProteccionMotores.cs   # Cálculo de protecciones de motores
│   ├── CalcularReactoresVFD.cs       # Diseño de reactores VFD
│   ├── TransformerForm.cs            # Diseño de transformadores
│   ├── VFD.cs                        # Análisis de variadores
│   ├── PresetForm.cs                 # Gestión de presets de diseño
│   └── SchematicViewerForm.cs        # Visor de esquemáticos
│
├── SchematicEditor/
│   ├── SchematicEditorForm.cs        # Editor principal de esquemáticos
│   ├── ComponentToolBox.cs           # Barra de herramientas de componentes
│   ├── SchematicComponent.cs         # Clase base para componentes
│   ├── SchematicDocument.cs          # Serialización JSON de esquemáticos
│   │
│   ├── Components/
│   │   ├── ResistorComponent.cs      # Componente resistor
│   │   ├── CapacitorComponent.cs     # Componente capacitor
│   │   ├── InductorComponent.cs      # Componente inductor
│   │   ├── WireComponent.cs          # Conexiones/cables
│   │   ├── TerminalComponent.cs      # Terminales/conectores
│   │   └── ICComponent.cs            # Circuitos integrados
│   │
│   └── Routing/
│       ├── RoutingEngine.cs          # Motor de ruteo de cables
│       ├── WaypointManager.cs        # Gestión de puntos intermedios
│       └── ConnectionManager.cs      # Gestión de conexiones
│
└── Resources/
    ├── Icons/                        # Iconos de la aplicación
    └── Templates/                    # Plantillas de circuitos
```

---

## Motor de Cálculo

### Clase `BuckCalculator`

Motor de cálculo completo para convertidores Buck implementado en `BuckCalculator.cs`.

#### Algoritmo de Cálculo

```csharp
public static CalculationResults Calculate(DesignParameters parameters)
```

**Pasos del algoritmo:**

1. **Fundamentos**
   - Ciclo de trabajo: `D = Vout / Vin`
   - Potencia salida: `Pout = Vout × Iout`
   - Potencia entrada: `Pin = Pout / η` (eficiencia inicial estimada)

2. **Diseño del Inductor**
   - Ripple de corriente: `ΔIL = Iout × RippleCurrentPercent / 100`
   - Inductancia mínima: `L = (Vin - Vout) × D / (ΔIL × f)`
   - Ajuste a valor comercial mediante `RoundToCommercialInductance()`
   - Corriente pico: `Ipk = Iout + ΔIL/2`
   - Corriente RMS: `Irms² = Iavg² + (ΔI²/12)`

3. **Diseño del Capacitor**
   - Capacitancia: `C = ΔIL / (8 × f × ΔVout)`
   - Ajuste a valor comercial mediante `RoundToCommercialCapacitance()`
   - ESR máximo: `ESRmax = ΔVout / ΔIL`
   - Corriente RMS del capacitor

4. **Red de Realimentación (UC3843)**
   - Tensión de referencia: `Vref = 2.5V`
   - Cálculo de divisor resistivo `R1/R2`
   - Ajuste de `R1` a serie E24

5. **Configuración PWM (UC3843)**
   - Capacitor típico: `Ct = 2.2nF`
   - Resistencia timing: `Rt = 1.72 / (f × Ct)`

6. **Análisis de Pérdidas**
   - Conducción MOSFET: `Pcond = Iout² × Rds(on) × D`
   - Switching MOSFET: `Psw = 0.5 × Vin × Iout × (tr + tf) × f`
   - Conducción diodo: `Pdiodo = Vf × Iout × (1 - D)`
   - Pérdidas núcleo inductor (estimadas)
   - Eficiencia real: `η = Pout / (Pout + Σ pérdidas)`

7. **Análisis Térmico**
   - Temperatura juntura MOSFET: `Tj = Ta + (Ptotal × θJA)`
   - Temperatura juntura diodo
   - Temperatura inductor

---

## Editor de Esquemáticos

### Clase `SchematicEditorForm`

Editor WYSIWYG completo para diseño de esquemáticos electrónicos.

#### Funcionalidades Principales

**Canvas y Navegación**
```csharp
- Zoom: Mouse wheel o botones +/-
- Pan: Clic medio + arrastrar
- Grid: 10px con snap-to-grid configurable
- Coordenadas absolutas y relativas al grid
```

**Herramientas de Dibujo**
- **Seleccionar**: Modo por defecto, selección múltiple con Ctrl
- **Resistor/Capacitor/Inductor**: Inserción rápida con valores por defecto
- **Terminal**: Puntos de conexión entrada/salida
- **Label**: Etiquetas de texto para identificación
- **Wire**: Herramienta de cableado con ruteo inteligente

**Modos de Ruteo de Cables**

```csharp
public enum RoutingMode
{
    Direct,      // Línea recta punto a punto
    Orthogonal,  // Manhattan routing (solo H/V)
    Auto         // Evitación automática (placeholder)
}
```

**Operaciones de Edición**
- `Copiar/Pegar`: Clipboard con offset automático
- `Duplicar`: Clonación rápida con offset
- `Eliminar`: Delete key o botón
- `Rotar`: 90° horario/antihorario
- `Espejar`: Horizontal/Vertical
- `Alinear`: Izquierda/Centro/Derecha, Superior/Medio/Inferior
- `Distribuir`: Horizontal/Vertical con espaciado uniforme

**Persistencia y Exportación**

```csharp
// Guardar esquemático
SchematicDocument.SaveToFile("diseño.json");

// Cargar esquemático
SchematicDocument.LoadFromFile("diseño.json");

// Exportar a imagen
ExportToPng("esquematico.png");
```

---

## Instalación

### Desde Código Fuente

```bash
# Clonar repositorio
git clone https://github.com/hfaalaniz/SuiteElectricaElectronica.git
cd SuiteElectricaElectronica/BuckConverterCalculator

# Restaurar dependencias
dotnet restore

# Compilar
dotnet build --configuration Release

# Ejecutar
dotnet run --project BuckConverterCalculator.csproj
```

### Desde Visual Studio

1. Abrir `BuckConverterCalculator.sln`
2. Configurar como proyecto de inicio
3. Presionar F5 para compilar y ejecutar

### Verificar Instalación de .NET 10

```bash
dotnet --version
# Debe mostrar: 10.x.x
```

---

## Uso

### Ejemplo 1: Diseño Básico Buck Converter

```csharp
using BuckConverterCalculator;

// Definir parámetros de diseño
var parameters = new DesignParameters
{
    Vin = 12.0,                    // 12V entrada
    Vout = 5.0,                    // 5V salida
    Iout = 3.0,                    // 3A corriente de carga
    Frequency = 100000,            // 100kHz switching
    RippleCurrentPercent = 20.0,   // 20% ripple corriente
    RippleVoltageMv = 50.0,        // 50mV ripple voltaje
    EfficiencyPercent = 90.0       // 90% eficiencia estimada
};

// Ejecutar cálculo
var results = BuckCalculator.Calculate(parameters);

// Resultados
Console.WriteLine($"Duty Cycle: {results.DutyCycle:P2}");
Console.WriteLine($"Inductancia: {results.InductanceCommercial * 1e6:F1} µH");
Console.WriteLine($"Capacitancia: {results.OutputCapacitanceCommercial * 1e6:F1} µF");
Console.WriteLine($"Eficiencia real: {results.ActualEfficiency:P2}");
```

**Salida esperada:**
```
Duty Cycle: 41.67%
Inductancia: 15.0 µH (ajustado de 14.6 µH)
Capacitancia: 100.0 µF (ajustado de 75 µF)
ESR máximo: 83.3 mΩ
Eficiencia real: 88.5%
```

### Ejemplo 2: Editor de Esquemáticos

```csharp
// Crear instancia del editor
var editor = new SchematicEditorForm();

// Cargar resultados del cálculo (opcional)
editor.LoadCalculationResults(results);

// Mostrar editor
editor.Show();

// El usuario puede:
// 1. Arrastrar componentes desde la librería
// 2. Usar ComponentToolBox para inserción rápida
// 3. Conectar componentes con la herramienta Wire
// 4. Editar propiedades en PropertyGrid
// 5. Guardar el esquemático como JSON
// 6. Exportar a PNG para documentación
```

---

## Modelos de Datos

### `DesignParameters` (Entrada)

```csharp
public class DesignParameters
{
    public double Vin { get; set; }                  // Voltaje entrada (V)
    public double Vout { get; set; }                 // Voltaje salida (V)
    public double Iout { get; set; }                 // Corriente salida (A)
    public double Frequency { get; set; }            // Frecuencia switching (Hz)
    public double RippleCurrentPercent { get; set; } // Ripple corriente (%)
    public double RippleVoltageMv { get; set; }      // Ripple voltaje (mV)
    public double EfficiencyPercent { get; set; }    // Eficiencia estimada (%)
}
```

### `CalculationResults` (Salida)

```csharp
public class CalculationResults
{
    // Fundamentales
    public double DutyCycle { get; set; }            // Ciclo de trabajo (0-1)
    public double PowerOutput { get; set; }          // Potencia salida (W)
    public double PowerInput { get; set; }           // Potencia entrada (W)
    public double ActualEfficiency { get; set; }     // Eficiencia real (%)
    
    // Inductor
    public double Inductance { get; set; }           // Inductancia calculada (H)
    public double InductanceCommercial { get; set; } // Valor comercial (H)
    public double RippleCurrent { get; set; }        // Ripple corriente (A)
    public double PeakInductorCurrent { get; set; }  // Corriente pico (A)
    public double RmsInductorCurrent { get; set; }   // Corriente RMS (A)
    
    // Capacitor
    public double OutputCapacitance { get; set; }           // Capacitancia calculada (F)
    public double OutputCapacitanceCommercial { get; set; } // Valor comercial (F)
    public double MaxEsr { get; set; }                      // ESR máximo (Ω)
    public double RmsCapacitorCurrent { get; set; }         // Corriente RMS (A)
    
    // Realimentación
    public double FeedbackR1 { get; set; }           // Resistencia R1 (Ω)
    public double FeedbackR2 { get; set; }           // Resistencia R2 (Ω)
    public double VoutVerified { get; set; }         // Vout verificado (V)
    
    // PWM (UC3843)
    public double RtValue { get; set; }              // Resistencia timing (Ω)
    public double CtValue { get; set; }              // Capacitor timing (F)
    public double ActualFrequency { get; set; }      // Frecuencia real (Hz)
    
    // Pérdidas
    public double MosfetConductionLoss { get; set; } // Pérdidas conducción (W)
    public double MosfetSwitchingLoss { get; set; }  // Pérdidas switching (W)
    public double MosfetTotalLoss { get; set; }      // Pérdidas totales MOSFET (W)
    public double DiodeConductionLoss { get; set; }  // Pérdidas diodo (W)
    public double InductorCoreLoss { get; set; }     // Pérdidas núcleo (W)
    public double TotalLosses { get; set; }          // Pérdidas totales (W)
    
    // Térmico
    public double MosfetJunctionTemp { get; set; }   // Temp. juntura MOSFET (°C)
    public double DiodeJunctionTemp { get; set; }    // Temp. juntura diodo (°C)
    public double InductorTemp { get; set; }         // Temp. inductor (°C)
}
```

---

## Fórmulas Implementadas

### Convertidor Buck - Ecuaciones Fundamentales

**Ciclo de Trabajo**
```
D = Vout / Vin
```

**Potencias**
```
Pout = Vout × Iout
Pin = Pout / η
```

**Inductancia Mínima (Modo CCM)**
```
L = (Vin - Vout) × D / (ΔIL × f)

donde:
  ΔIL = Iout × (RippleCurrentPercent / 100)
```

**Corrientes del Inductor**
```
Ipico = Iout + ΔIL/2
Ivalle = Iout - ΔIL/2

Irms² = Iavg² + (ΔIL²/12)
```

**Capacitor de Salida**
```
C = ΔIL / (8 × f × ΔVout)

ESRmax = ΔVout / ΔIL

donde:
  ΔVout = RippleVoltageMv / 1000
```

**Corriente RMS del Capacitor**
```
Icap_rms ≈ ΔIL / (2√3)
```

### Red de Realimentación (UC3843)

**Divisor Resistivo**
```
Vout = Vref × (1 + R1/R2)

donde:
  Vref = 2.5V (UC3843)
  
Resolviendo:
  R2 = R1 / ((Vout/Vref) - 1)
```

### Configuración PWM (UC3843)

**Temporización**
```
f = 1.72 / (Rt × Ct)

Rt = 1.72 / (f × Ct)

donde:
  Ct típico = 2.2nF
```

### Análisis de Pérdidas

**MOSFET - Conducción**
```
Pcond = Iout² × Rds(on) × D

donde:
  Rds(on) típico: 10-100mΩ (depende del MOSFET)
```

**MOSFET - Switching**
```
Psw = 0.5 × Vin × Iout × (tr + tf) × f

donde:
  tr + tf ≈ 50-200ns (tiempos de conmutación)
```

**Diodo - Conducción**
```
Pdiodo = Vf × Iout × (1 - D)

donde:
  Vf ≈ 0.4-0.7V (diodo Schottky típico)
```

**Inductor - Núcleo**
```
Pcore ≈ k × f^α × B^β × Volumen

(fórmula de Steinmetz, estimación simplificada)
```

**Eficiencia Real**
```
η = Pout / (Pout + ΣPérdidas) × 100%
```

### Análisis Térmico

**Temperatura de Juntura**
```
Tj = Ta + (Pdis × θJA)

donde:
  Ta = temperatura ambiente (25°C típico)
  θJA = resistencia térmica juntura-ambiente (°C/W)
  Pdis = potencia disipada (W)
```

---

## Módulos Adicionales

### Cálculo de Protección de Motores

**Archivo:** `CalculoProteccionMotores.cs`

Dimensionado de protecciones térmicas y magnéticas para motores trifásicos.

**Funcionalidades:**
- Cálculo de corriente nominal del motor
- Selección de relé térmico
- Ajuste de rango de corriente
- Factor de servicio
- Tiempo de disparo

### Cálculo de Reactores VFD

**Archivo:** `CalcularReactoresVFD.cs`

Diseño de reactores de línea y carga para variadores de frecuencia.

**Funcionalidades:**
- Reactor de línea (% impedancia)
- Reactor de carga (dv/dt, reducción armónicos)
- Dimensionado de núcleo
- Calibre de conductor
- Pérdidas y temperatura

### Diseño de Transformadores

**Archivo:** `TransformerForm.cs`

Cálculo completo de transformadores monofásicos y trifásicos.

**Funcionalidades:**
- Potencia y relación de transformación
- Número de espiras primario/secundario
- Área del núcleo (Ae)
- Calibre de conductores
- Pérdidas cobre y hierro
- Regulación y eficiencia

### Análisis VFD

**Archivo:** `VFD.cs`

Herramientas para análisis de variadores de frecuencia.

**Funcionalidades:**
- Curvas V/f
- Cálculo de par motor
- Análisis de armónicos
- Factor de potencia
- Eficiencia del sistema

---

## Desarrollo

### Configuración del Entorno

```bash
# Instalar .NET 10 SDK
https://dotnet.microsoft.com/download/dotnet/10.0

# Verificar instalación
dotnet --version

# Clonar repositorio
git clone https://github.com/hfaalaniz/SuiteElectricaElectronica.git
cd SuiteElectricaElectronica/BuckConverterCalculator
```

### Compilación

```bash
# Debug
dotnet build --configuration Debug

# Release
dotnet build --configuration Release

# Publicar
dotnet publish --configuration Release --output ./publish
```

### Estructura de Branches

- `main` — Rama principal estable
- `develop` — Desarrollo activo
- `feature/*` — Nuevas características
- `bugfix/*` — Correcciones de bugs

### Convenciones de Código

- **Lenguaje:** C# 14.0
- **Formato:** 4 espacios de indentación
- **Naming:** PascalCase para clases/métodos, camelCase para variables
- **Comentarios:** XML documentation para APIs públicas

```csharp
/// <summary>
/// Calcula los parámetros del convertidor Buck.
/// </summary>
/// <param name="parameters">Parámetros de diseño</param>
/// <returns>Resultados del cálculo</returns>
public static CalculationResults Calculate(DesignParameters parameters)
{
    // Implementación
}
```

### Testing

```bash
# Ejecutar tests unitarios (cuando estén disponibles)
dotnet test

# Ejecutar con cobertura
dotnet test /p:CollectCoverage=true
```

---

## Roadmap

### Versión 2.0 (Próxima)
- [ ] Soporte para modo DCM (Discontinuous Conduction Mode)
- [ ] Análisis de estabilidad (diagrama de Bode)
- [ ] Simulación temporal de formas de onda
- [ ] Base de datos de componentes comerciales integrada
- [ ] Exportación de BOM (Bill of Materials)
- [ ] Generación de PCB layout básico

### Versión 2.1
- [ ] Soporte para otros topologías (Boost, Buck-Boost, Flyback)
- [ ] Análisis de EMI/EMC
- [ ] Optimización multiobjetivo (tamaño vs eficiencia)
- [ ] Integración con SPICE para simulación
- [ ] Templates de circuitos predefinidos

### Versión 3.0
- [ ] Editor de PCB completo
- [ ] Análisis térmico CFD
- [ ] Integración con manufactureras (cotización automática)
- [ ] Modo colaborativo en red
- [ ] Plugin system para extensiones

---

## Contribuir

### Cómo Contribuir

1. **Fork** del repositorio
2. **Crear rama** de feature
   ```bash
   git checkout -b feature/NuevaFuncionalidad
   ```
3. **Commit** de cambios
   ```bash
   git commit -m "Add: Cálculo de eficiencia mejorado"
   ```
4. **Push** a la rama
   ```bash
   git push origin feature/NuevaFuncionalidad
   ```
5. **Crear Pull Request** con descripción detallada

### Guía de Estilo

- Seguir convenciones de código C#
- Incluir tests para nueva funcionalidad
- Documentar APIs públicas con XML comments
- Actualizar README si es necesario

### Reportar Bugs

Crear un [Issue](https://github.com/hfaalaniz/SuiteElectricaElectronica/issues) con:
- Descripción del problema
- Pasos para reproducir
- Comportamiento esperado vs actual
- Screenshots si aplica
- Información del sistema (OS, .NET version)

---

## Licencia

Este proyecto es parte de **SuiteElectricaElectronica**.

**Licencia:** [MIT/Apache 2.0/GPL]

---

## Autor

**Hugo Fabián Alaniz**
- GitHub: [@hfaalaniz](https://github.com/hfaalaniz)
- Email: [hfaalaniz@gmail.com]

---

## ☕ Apoya este Proyecto

Si este proyecto te ha sido útil y te ha ahorrado tiempo en tus diseños de electrónica de potencia, considera invitarme un café. Tu apoyo ayuda a mantener el proyecto activo y a desarrollar nuevas funcionalidades.

[![Buy Me A Coffee](https://img.shields.io/badge/Buy%20Me%20A%20Coffee-Support-yellow?style=for-the-badge&logo=buy-me-a-coffee)](https://www.buymeacoffee.com/hfaalaniz)

**Otras formas de apoyar:**
- ⭐ Dale una estrella al repositorio en GitHub
- 🐛 Reporta bugs y sugiere mejoras
- 📖 Contribuye con código o documentación
- 💬 Comparte el proyecto con otros ingenieros

**Tu apoyo hace la diferencia** y permite seguir mejorando esta suite de herramientas para la comunidad de ingeniería electrónica.

---

## Referencias y Bibliografía

### Libros de Referencia
- **"Fundamentals of Power Electronics" (3rd Ed.)** — Robert W. Erickson, Dragan Maksimović
- **"Power Electronics: Converters, Applications, and Design" (3rd Ed.)** — Ned Mohan, Tore M. Undeland, William P. Robbins
- **"Switching Power Supply Design" (3rd Ed.)** — Abraham I. Pressman

### Hojas de Datos
- **UC3843/UC3844** — Texas Instruments (PWM Controller)
- **IRF540N** — Infineon (MOSFET de potencia ejemplo)
- **MBR20100CT** — Vishay (Diodo Schottky ejemplo)

### Normas Aplicables
- **IEC 61000-3-2** — Límites de emisión de armónicos
- **IEC 61000-4-4** — Inmunidad a transitorios eléctricos
- **EN 55022** — Compatibilidad electromagnética

### Recursos Online
- [TI Power Design Tool](https://www.ti.com/design-tools-simulation/design-tools-software.html)
- [Analog Devices Power Design](https://www.analog.com/en/design-center/design-tools-and-calculators.html)
- [Würth Elektronik REDEXPERT](https://redexpert.we-online.com/)

---

## Contacto y Soporte

### Soporte Técnico
- **Issues:** [GitHub Issues](https://github.com/hfaalaniz/SuiteElectricaElectronica/issues)
- **Discusiones:** [GitHub Discussions](https://github.com/hfaalaniz/SuiteElectricaElectronica/discussions)

### Comunidad
- Reportar bugs o solicitar features vía GitHub Issues
- Contribuir con código mediante Pull Requests
- Compartir diseños y casos de uso en Discussions

---

**Parte de:** [SuiteElectricaElectronica](https://github.com/hfaalaniz/SuiteElectricaElectronica)

**Última actualización:** Enero 2026

---

## Agradecimientos

Agradecimientos especiales a la comunidad de electrónica de potencia y desarrolladores de software open source que hacen posible proyectos como este.

**Made with ❤️ for Electronics Engineers**
