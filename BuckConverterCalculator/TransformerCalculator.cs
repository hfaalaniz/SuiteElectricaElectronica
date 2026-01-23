using System;
using System.Collections.Generic;

namespace BuckConverterCalculator

{
    public enum TipoConexion
    {
        Monofasico,
        Bifasico,
        TrifasicoEstrella,
        TrifasicoDelta
    }

    public enum TipoLaminado
    {
        M19_29Gauge,      // 0.35mm - Estándar
        M15_29Gauge,      // 0.35mm - Bajo pérdidas
        M6_27Gauge,       // 0.40mm - Muy bajo pérdidas
        Amorphous,        // Amorfo - Mínimas pérdidas
        GrainOriented     // Grano orientado - Alta eficiencia
    }

    public class LaminadoData
    {
        public double Espesor { get; set; }           // mm
        public double DensidadFlujoMax { get; set; }  // Tesla
        public double PerdidasNucleo { get; set; }    // W/kg a 60Hz, 1.5T
        public double DensidadAcero { get; set; }     // g/cm³
        public double FactorApilamiento { get; set; }
        public string Nombre { get; set; }
    }

    public class TransformerConfig
    {
        public TipoConexion TipoPrimario { get; set; }
        public TipoConexion TipoSecundario { get; set; }
        public double VoltajePrimario { get; set; }
        public double VoltajeSecundario { get; set; }
        public double CorrienteSecundaria { get; set; }
        public double Frecuencia { get; set; }
        public double Eficiencia { get; set; }
        public TipoLaminado TipoLaminado { get; set; }
        public double DensidadCorriente { get; set; }
        public double FactorLlenado { get; set; }
        public double TempAmbiente { get; set; }
        public double ElevacionTemp { get; set; }
    }

    public class TransformerResult
    {
        public string TipoPrimario { get; set; }
        public string TipoSecundario { get; set; }
        public double PotenciaSecundaria { get; set; }
        public double PotenciaPrimaria { get; set; }
        public double CorrientePrimaria { get; set; }
        public double CorrienteSecundaria { get; set; }
        public double CorrientePrimarioFase { get; set; }
        public double CorrienteSecundarioFase { get; set; }
        public double VoltajePrimarioFase { get; set; }
        public double VoltajeSecundarioFase { get; set; }
        public double RelacionTransformacion { get; set; }
        public double RelacionCorrientes { get; set; }

        // Núcleo
        public double AreaNucleoEfectiva { get; set; }
        public double AreaNucleoBruta { get; set; }
        public double FlujoMaximo { get; set; }
        public double InduccionReal { get; set; }
        public double VoltioPorEspira { get; set; }
        public int EspirasPrimario { get; set; }
        public int EspirasSecundario { get; set; }

        // Laminado
        public string NombreLaminado { get; set; }
        public double EspesorLaminado { get; set; }
        public double PesoNucleo { get; set; }
        public double PerdidasNucleo { get; set; }

        // Conductores
        public int FasesPrimario { get; set; }
        public int FasesSecundario { get; set; }
        public string CalibrePrimario { get; set; }
        public string CalibreSecundario { get; set; }
        public double AreaConductorPrimario { get; set; }
        public double AreaConductorSecundario { get; set; }
        public double DiametroConductorPrimario { get; set; }
        public double DiametroConductorSecundario { get; set; }

        // Devanados
        public double LongitudMediaEspira { get; set; }
        public double LongitudCobrePrimario { get; set; }
        public double LongitudCobreSecundario { get; set; }
        public double PesoCobrePrimario { get; set; }
        public double PesoCobreSecundario { get; set; }
        public double ResistenciaPrimario { get; set; }
        public double ResistenciaSecundario { get; set; }

        // Pérdidas y temperatura
        public double PerdidasCobre { get; set; }
        public double PerdidasTotales { get; set; }
        public double EficienciaReal { get; set; }
        public double RegulacionVoltaje { get; set; }
        public double TempDevanados { get; set; }

        // Ventana
        public double AreaVentanaRequerida { get; set; }
        public double ProductoAp { get; set; }

        // Dimensiones físicas
        public double AnchoNucleo { get; set; }
        public double AltoNucleo { get; set; }
        public double AnchoVentana { get; set; }
        public double AltoVentana { get; set; }
        public double PesoTotal { get; set; }
    }

    public class TransformerCalculator
    {
        private const double SQRT3 = 1.7320508075688772935;
        private const double PI = 3.1415926535897932384626;
        private const double DENSIDAD_COBRE = 8.96;  // g/cm³
        private const double RESISTIVIDAD_COBRE = 1.724e-8; // Ohm·m a 20°C
        private const double COEF_TEMP_COBRE = 0.00393; // 1/°C

        private static readonly Dictionary<TipoLaminado, LaminadoData> LaminadosDB = new()
        {
            { TipoLaminado.M19_29Gauge, new LaminadoData {
                Espesor = 0.35, DensidadFlujoMax = 1.5, PerdidasNucleo = 3.5,
                DensidadAcero = 7.65, FactorApilamiento = 0.95, Nombre = "M19 (29ga) - 0.35mm"
            }},
            { TipoLaminado.M15_29Gauge, new LaminadoData {
                Espesor = 0.35, DensidadFlujoMax = 1.6, PerdidasNucleo = 2.3,
                DensidadAcero = 7.65, FactorApilamiento = 0.95, Nombre = "M15 (29ga) - 0.35mm"
            }},
            { TipoLaminado.M6_27Gauge, new LaminadoData {
                Espesor = 0.40, DensidadFlujoMax = 1.65, PerdidasNucleo = 1.5,
                DensidadAcero = 7.65, FactorApilamiento = 0.94, Nombre = "M6 (27ga) - 0.40mm"
            }},
            { TipoLaminado.Amorphous, new LaminadoData {
                Espesor = 0.025, DensidadFlujoMax = 1.4, PerdidasNucleo = 0.3,
                DensidadAcero = 7.18, FactorApilamiento = 0.88, Nombre = "Amorfo - 0.025mm"
            }},
            { TipoLaminado.GrainOriented, new LaminadoData {
                Espesor = 0.30, DensidadFlujoMax = 1.7, PerdidasNucleo = 1.1,
                DensidadAcero = 7.65, FactorApilamiento = 0.96, Nombre = "Grano Orientado - 0.30mm"
            }}
        };

        public TransformerResult Calculate(TransformerConfig config)
        {
            ValidarConfiguracion(config);

            var lam = LaminadosDB[config.TipoLaminado];
            var result = new TransformerResult
            {
                TipoPrimario = GetNombreTipo(config.TipoPrimario),
                TipoSecundario = GetNombreTipo(config.TipoSecundario),
                FasesPrimario = GetNumeroFases(config.TipoPrimario),
                FasesSecundario = GetNumeroFases(config.TipoSecundario),
                NombreLaminado = lam.Nombre,
                EspesorLaminado = lam.Espesor
            };

            // PASO 1: Potencias
            result.PotenciaSecundaria = CalcularPotenciaAparente(
                config.VoltajeSecundario, config.CorrienteSecundaria, config.TipoSecundario);

            double efDecimal = config.Eficiencia / 100.0;
            result.PotenciaPrimaria = result.PotenciaSecundaria / efDecimal;

            // PASO 2: Voltajes y corrientes
            result.VoltajePrimarioFase = CalcularVoltajeFase(config.VoltajePrimario, config.TipoPrimario);
            result.VoltajeSecundarioFase = CalcularVoltajeFase(config.VoltajeSecundario, config.TipoSecundario);

            result.CorrientePrimaria = CalcularCorrienteLinea(result.PotenciaPrimaria, config.VoltajePrimario, config.TipoPrimario);
            result.CorrienteSecundaria = config.CorrienteSecundaria;

            result.CorrientePrimarioFase = CalcularCorrienteFase(result.CorrientePrimaria, config.TipoPrimario);
            result.CorrienteSecundarioFase = CalcularCorrienteFase(result.CorrienteSecundaria, config.TipoSecundario);

            // PASO 3: Relaciones
            result.RelacionTransformacion = result.VoltajePrimarioFase / result.VoltajeSecundarioFase;
            result.RelacionCorrientes = result.CorrienteSecundarioFase / result.CorrientePrimarioFase;

            // PASO 4: Área del núcleo (Método Kg simplificado mejorado)
            // Kg = constante de potencia aparente ≈ 2.22 × f × Bmax × J × Ku
            double Kg = 2.22 * config.Frecuencia * lam.DensidadFlujoMax * config.DensidadCorriente * config.FactorLlenado;

            // Potencia total del transformador
            double potTotal = Math.Max(result.PotenciaPrimaria, result.PotenciaSecundaria);

            // Ae en cm²: Ae = sqrt(Pt / Kg) × 100
            result.AreaNucleoEfectiva = Math.Sqrt(potTotal / Kg) * 100.0;
            result.AreaNucleoBruta = result.AreaNucleoEfectiva / lam.FactorApilamiento;

            // PASO 5: Flujo magnético real
            double areaNucleoM2 = result.AreaNucleoEfectiva * 1e-4;
            result.FlujoMaximo = lam.DensidadFlujoMax * areaNucleoM2;
            result.InduccionReal = lam.DensidadFlujoMax;

            // PASO 6: Voltios por espira (Ley de Faraday)
            // E = 4.44 × f × Φmax
            result.VoltioPorEspira = 4.44 * config.Frecuencia * result.FlujoMaximo;

            // PASO 7: Número de espiras
            result.EspirasPrimario = (int)Math.Round(result.VoltajePrimarioFase / result.VoltioPorEspira);
            result.EspirasSecundario = (int)Math.Round(result.VoltajeSecundarioFase / result.VoltioPorEspira);

            if (result.EspirasPrimario < 1) result.EspirasPrimario = 1;
            if (result.EspirasSecundario < 1) result.EspirasSecundario = 1;

            // PASO 8: Conductores
            result.AreaConductorPrimario = result.CorrientePrimarioFase / config.DensidadCorriente;
            result.AreaConductorSecundario = result.CorrienteSecundarioFase / config.DensidadCorriente;

            result.CalibrePrimario = CalcularCalibreAWG(result.AreaConductorPrimario, out double areaPrim, out double diamPrim);
            result.CalibreSecundario = CalcularCalibreAWG(result.AreaConductorSecundario, out double areaSec, out double diamSec);

            result.AreaConductorPrimario = areaPrim;
            result.AreaConductorSecundario = areaSec;
            result.DiametroConductorPrimario = diamPrim;
            result.DiametroConductorSecundario = diamSec;

            // PASO 9: Dimensiones físicas del núcleo (núcleo tipo EI)
            result.AnchoNucleo = Math.Sqrt(result.AreaNucleoEfectiva);
            result.AltoNucleo = result.AnchoNucleo * 2.0;

            // Ventana (aproximación 50% del alto del núcleo)
            result.AltoVentana = result.AltoNucleo * 0.5;
            result.AnchoVentana = result.AnchoNucleo;

            // PASO 10: Longitud media de espira
            result.LongitudMediaEspira = 2.0 * (result.AnchoNucleo + result.AltoVentana) + PI * result.AnchoNucleo * 0.5;

            // Longitudes totales
            result.LongitudCobrePrimario = result.LongitudMediaEspira * result.EspirasPrimario * result.FasesPrimario / 100.0; // metros
            result.LongitudCobreSecundario = result.LongitudMediaEspira * result.EspirasSecundario * result.FasesSecundario / 100.0;

            // PASO 11: Pesos
            double volCobrePrim = result.AreaConductorPrimario * result.LongitudCobrePrimario * 100.0; // cm³
            double volCobreSec = result.AreaConductorSecundario * result.LongitudCobreSecundario * 100.0;

            result.PesoCobrePrimario = volCobrePrim * DENSIDAD_COBRE;
            result.PesoCobreSecundario = volCobreSec * DENSIDAD_COBRE;

            // Peso del núcleo (volumen aproximado)
            double volNucleo = result.AreaNucleoBruta * (2.0 * result.AnchoNucleo + result.AltoNucleo);
            result.PesoNucleo = volNucleo * lam.DensidadAcero;
            result.PesoTotal = result.PesoNucleo + result.PesoCobrePrimario + result.PesoCobreSecundario;

            // PASO 12: Resistencias (a temperatura de trabajo)
            double tempTrabajo = config.TempAmbiente + config.ElevacionTemp;
            double factorTemp = 1.0 + COEF_TEMP_COBRE * (tempTrabajo - 20.0);

            result.ResistenciaPrimario = RESISTIVIDAD_COBRE * factorTemp * result.LongitudCobrePrimario / (result.AreaConductorPrimario * 1e-6);
            result.ResistenciaSecundario = RESISTIVIDAD_COBRE * factorTemp * result.LongitudCobreSecundario / (result.AreaConductorSecundario * 1e-6);

            // PASO 13: Pérdidas
            result.PerdidasNucleo = lam.PerdidasNucleo * (result.PesoNucleo / 1000.0) * Math.Pow(result.InduccionReal / 1.5, 2.0);

            double perdidasCobrePrim = Math.Pow(result.CorrientePrimarioFase, 2) * result.ResistenciaPrimario * result.FasesPrimario;
            double perdidasCobreSec = Math.Pow(result.CorrienteSecundarioFase, 2) * result.ResistenciaSecundario * result.FasesSecundario;
            result.PerdidasCobre = perdidasCobrePrim + perdidasCobreSec;

            result.PerdidasTotales = result.PerdidasNucleo + result.PerdidasCobre;
            result.EficienciaReal = (result.PotenciaSecundaria / (result.PotenciaSecundaria + result.PerdidasTotales)) * 100.0;

            // PASO 14: Regulación de voltaje
            result.RegulacionVoltaje = ((result.ResistenciaSecundario * result.CorrienteSecundarioFase) / result.VoltajeSecundarioFase) * 100.0;

            // PASO 15: Temperatura
            result.TempDevanados = config.TempAmbiente + config.ElevacionTemp;

            // PASO 16: Ventana
            double volCondPrim = areaPrim * result.EspirasPrimario * result.FasesPrimario;
            double volCondSec = areaSec * result.EspirasSecundario * result.FasesSecundario;
            result.AreaVentanaRequerida = (volCondPrim + volCondSec) / (config.FactorLlenado * 100.0);
            result.ProductoAp = result.AreaNucleoEfectiva * result.AreaVentanaRequerida;

            return result;
        }

        private void ValidarConfiguracion(TransformerConfig config)
        {
            if (config.VoltajePrimario <= 0) throw new ArgumentException("Voltaje primario inválido");
            if (config.VoltajeSecundario <= 0) throw new ArgumentException("Voltaje secundario inválido");
            if (config.CorrienteSecundaria <= 0 || config.CorrienteSecundaria > 50)
                throw new ArgumentException("Corriente secundaria debe estar entre 0 y 50A");
            if (config.Frecuencia <= 0) throw new ArgumentException("Frecuencia inválida");
            if (config.Eficiencia <= 0 || config.Eficiencia > 100) throw new ArgumentException("Eficiencia debe estar entre 0 y 100%");
            if (config.DensidadCorriente <= 0) throw new ArgumentException("Densidad de corriente inválida");
            if (config.FactorLlenado <= 0 || config.FactorLlenado > 1) throw new ArgumentException("Factor de llenado debe estar entre 0 y 1");
        }

        private double CalcularPotenciaAparente(double v, double i, TipoConexion tipo)
        {
            return tipo switch
            {
                TipoConexion.Monofasico => v * i,
                TipoConexion.Bifasico => 2.0 * v * i,
                TipoConexion.TrifasicoEstrella or TipoConexion.TrifasicoDelta => SQRT3 * v * i,
                _ => v * i
            };
        }

        private double CalcularCorrienteLinea(double p, double v, TipoConexion tipo)
        {
            return tipo switch
            {
                TipoConexion.Monofasico => p / v,
                TipoConexion.Bifasico => p / (2.0 * v),
                TipoConexion.TrifasicoEstrella or TipoConexion.TrifasicoDelta => p / (SQRT3 * v),
                _ => p / v
            };
        }

        private double CalcularVoltajeFase(double vl, TipoConexion tipo)
        {
            return tipo switch
            {
                TipoConexion.TrifasicoEstrella => vl / SQRT3,
                _ => vl
            };
        }

        private double CalcularCorrienteFase(double il, TipoConexion tipo)
        {
            return tipo switch
            {
                TipoConexion.TrifasicoDelta => il / SQRT3,
                _ => il
            };
        }

        private string GetNombreTipo(TipoConexion tipo)
        {
            return tipo switch
            {
                TipoConexion.Monofasico => "Monofásico (1φ)",
                TipoConexion.Bifasico => "Bifásico (2φ-90°)",
                TipoConexion.TrifasicoEstrella => "Trifásico Estrella (3φ-Y)",
                TipoConexion.TrifasicoDelta => "Trifásico Delta (3φ-Δ)",
                _ => "Desconocido"
            };
        }

        private int GetNumeroFases(TipoConexion tipo)
        {
            return tipo switch
            {
                TipoConexion.Monofasico => 1,
                TipoConexion.Bifasico => 2,
                TipoConexion.TrifasicoEstrella or TipoConexion.TrifasicoDelta => 3,
                _ => 1
            };
        }

        private string CalcularCalibreAWG(double areaReq, out double areaReal, out double diametro)
        {
            var awg = new[]
            {
                (n: "4/0", a: 107.2193, d: 11.684), (n: "3/0", a: 85.0288, d: 10.405),
                (n: "2/0", a: 67.4309, d: 9.266), (n: "1/0", a: 53.4751, d: 8.252),
                (n: "1", a: 42.4077, d: 7.348), (n: "2", a: 33.6308, d: 6.544),
                (n: "3", a: 26.6705, d: 5.827), (n: "4", a: 21.1506, d: 5.189),
                (n: "5", a: 16.7732, d: 4.621), (n: "6", a: 13.3018, d: 4.115),
                (n: "7", a: 10.5488, d: 3.665), (n: "8", a: 8.3656, d: 3.264),
                (n: "9", a: 6.6342, d: 2.906), (n: "10", a: 5.2612, d: 2.588),
                (n: "11", a: 4.1723, d: 2.305), (n: "12", a: 3.3088, d: 2.053),
                (n: "13", a: 2.6240, d: 1.828), (n: "14", a: 2.0809, d: 1.628),
                (n: "15", a: 1.6502, d: 1.450), (n: "16", a: 1.3087, d: 1.291),
                (n: "17", a: 1.0378, d: 1.150), (n: "18", a: 0.8230, d: 1.024),
                (n: "19", a: 0.6527, d: 0.912), (n: "20", a: 0.5176, d: 0.812),
                (n: "21", a: 0.4105, d: 0.723), (n: "22", a: 0.3255, d: 0.644),
                (n: "23", a: 0.2582, d: 0.573), (n: "24", a: 0.2047, d: 0.511)
            };

            foreach (var (n, a, d) in awg)
            {
                if (a >= areaReq)
                {
                    areaReal = a;
                    diametro = d;
                    return n;
                }
            }

            int np = (int)Math.Ceiling(areaReq / 107.2193);
            areaReal = np * 107.2193;
            diametro = 11.684;
            return $"{np}×4/0";
        }
    }
}