using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;

namespace BuckConverterCalculator.Analysis
{
    /// <summary>
    /// Representa una función de transferencia H(s) en el dominio de Laplace
    /// </summary>
    public class TransferFunction
    {
        /// <summary>
        /// Coeficientes del numerador (orden descendente: a_n*s^n + ... + a_1*s + a_0)
        /// </summary>
        public double[] NumeratorCoefficients { get; set; }

        /// <summary>
        /// Coeficientes del denominador (orden descendente)
        /// </summary>
        public double[] DenominatorCoefficients { get; set; }

        /// <summary>
        /// Ganancia DC (valor en s=0)
        /// </summary>
        public double DCGain
        {
            get
            {
                if (DenominatorCoefficients.Last() == 0)
                    return double.PositiveInfinity;

                return NumeratorCoefficients.Last() / DenominatorCoefficients.Last();
            }
        }

        public TransferFunction(double[] numerator, double[] denominator)
        {
            this.NumeratorCoefficients = numerator;
            this.DenominatorCoefficients = denominator;
        }

        /// <summary>
        /// Evalúa la función de transferencia en un punto s complejo
        /// </summary>
        public Complex Evaluate(Complex s)
        {
            Complex numerator = EvaluatePolynomial(NumeratorCoefficients, s);
            Complex denominator = EvaluatePolynomial(DenominatorCoefficients, s);

            if (denominator.Magnitude < 1e-10)
                return Complex.Zero;

            return numerator / denominator;
        }

        /// <summary>
        /// Evalúa la función de transferencia a una frecuencia específica
        /// </summary>
        public Complex EvaluateAtFrequency(double frequency)
        {
            double omega = 2 * Math.PI * frequency;
            Complex s = new Complex(0, omega);
            return Evaluate(s);
        }

        /// <summary>
        /// Calcula la magnitud en dB a una frecuencia
        /// </summary>
        public double GetMagnitudeDB(double frequency)
        {
            var response = EvaluateAtFrequency(frequency);
            return 20 * Math.Log10(response.Magnitude);
        }

        /// <summary>
        /// Calcula la fase en grados a una frecuencia
        /// </summary>
        public double GetPhaseDegrees(double frequency)
        {
            var response = EvaluateAtFrequency(frequency);
            return response.Phase * 180 / Math.PI;
        }

        /// <summary>
        /// Calcula los polos de la función de transferencia
        /// </summary>
        public List<Complex> CalculatePoles()
        {
            return FindRoots(DenominatorCoefficients);
        }

        /// <summary>
        /// Calcula los ceros de la función de transferencia
        /// </summary>
        public List<Complex> CalculateZeros()
        {
            return FindRoots(NumeratorCoefficients);
        }

        /// <summary>
        /// Verifica si el sistema es estable (todos los polos en semiplano izquierdo)
        /// </summary>
        public bool IsStable()
        {
            var poles = CalculatePoles();
            return poles.All(p => p.Real < 0);
        }

        /// <summary>
        /// Calcula la respuesta en frecuencia
        /// </summary>
        public FrequencyResponse GetFrequencyResponse(double[] frequencies)
        {
            var response = new FrequencyResponse
            {
                Frequencies = frequencies.ToList(),
                Magnitude = new List<double>(),
                Phase = new List<double>()
            };

            foreach (var freq in frequencies)
            {
                var h = EvaluateAtFrequency(freq);
                response.Magnitude.Add(20 * Math.Log10(h.Magnitude));
                response.Phase.Add(h.Phase * 180 / Math.PI);
            }

            return response;
        }

        /// <summary>
        /// Multiplica dos funciones de transferencia (cascada)
        /// </summary>
        public static TransferFunction Multiply(TransferFunction tf1, TransferFunction tf2)
        {
            var numerator = MultiplyPolynomials(tf1.NumeratorCoefficients, tf2.NumeratorCoefficients);
            var denominator = MultiplyPolynomials(tf1.DenominatorCoefficients, tf2.DenominatorCoefficients);

            return new TransferFunction(numerator, denominator);
        }

        /// <summary>
        /// Suma dos funciones de transferencia (paralelo)
        /// </summary>
        public static TransferFunction Add(TransferFunction tf1, TransferFunction tf2)
        {
            // H1 + H2 = (N1*D2 + N2*D1) / (D1*D2)
            var n1d2 = MultiplyPolynomials(tf1.NumeratorCoefficients, tf2.DenominatorCoefficients);
            var n2d1 = MultiplyPolynomials(tf2.NumeratorCoefficients, tf1.DenominatorCoefficients);
            var numerator = AddPolynomials(n1d2, n2d1);
            var denominator = MultiplyPolynomials(tf1.DenominatorCoefficients, tf2.DenominatorCoefficients);

            return new TransferFunction(numerator, denominator);
        }

        /// <summary>
        /// Función de transferencia de realimentación
        /// </summary>
        public static TransferFunction Feedback(TransferFunction forward, TransferFunction feedback)
        {
            // H_closed = G / (1 + G*H)
            var gh = Multiply(forward, feedback);

            // Numerator: G_num * (1 + GH)_den
            var numerator = MultiplyPolynomials(
                forward.NumeratorCoefficients,
                AddPolynomials(new double[] { 1 }, gh.DenominatorCoefficients)
            );

            // Denominator: G_den * (1 + GH)_num
            var denominator = MultiplyPolynomials(
                forward.DenominatorCoefficients,
                AddPolynomials(gh.NumeratorCoefficients, gh.DenominatorCoefficients)
            );

            return new TransferFunction(numerator, denominator);
        }

        #region Polynomial Operations

        private static Complex EvaluatePolynomial(double[] coefficients, Complex s)
        {
            Complex result = Complex.Zero;
            int order = coefficients.Length - 1;

            for (int i = 0; i < coefficients.Length; i++)
            {
                int power = order - i;
                result += coefficients[i] * Complex.Pow(s, power);
            }

            return result;
        }

        private static double[] MultiplyPolynomials(double[] p1, double[] p2)
        {
            int resultLength = p1.Length + p2.Length - 1;
            double[] result = new double[resultLength];

            for (int i = 0; i < p1.Length; i++)
            {
                for (int j = 0; j < p2.Length; j++)
                {
                    result[i + j] += p1[i] * p2[j];
                }
            }

            return result;
        }

        private static double[] AddPolynomials(double[] p1, double[] p2)
        {
            int maxLength = Math.Max(p1.Length, p2.Length);
            double[] result = new double[maxLength];

            // Alinear por la derecha (coeficientes de menor orden)
            int offset1 = maxLength - p1.Length;
            int offset2 = maxLength - p2.Length;

            for (int i = 0; i < p1.Length; i++)
                result[offset1 + i] += p1[i];

            for (int i = 0; i < p2.Length; i++)
                result[offset2 + i] += p2[i];

            return result;
        }

        /// <summary>
        /// Encuentra las raíces de un polinomio (método numérico simple)
        /// </summary>
        private List<Complex> FindRoots(double[] coefficients)
        {
            var roots = new List<Complex>();

            if (coefficients.Length == 1)
                return roots; // Constante, sin raíces

            if (coefficients.Length == 2)
            {
                // Linear: ax + b = 0 => x = -b/a
                if (coefficients[0] != 0)
                    roots.Add(new Complex(-coefficients[1] / coefficients[0], 0));
                return roots;
            }

            if (coefficients.Length == 3)
            {
                // Quadratic: ax^2 + bx + c = 0
                double a = coefficients[0];
                double b = coefficients[1];
                double c = coefficients[2];

                double discriminant = b * b - 4 * a * c;

                if (discriminant >= 0)
                {
                    roots.Add(new Complex((-b + Math.Sqrt(discriminant)) / (2 * a), 0));
                    roots.Add(new Complex((-b - Math.Sqrt(discriminant)) / (2 * a), 0));
                }
                else
                {
                    double real = -b / (2 * a);
                    double imag = Math.Sqrt(-discriminant) / (2 * a);
                    roots.Add(new Complex(real, imag));
                    roots.Add(new Complex(real, -imag));
                }

                return roots;
            }

            // Para polinomios de orden superior, usar método numérico
            // (implementación simplificada - en producción usar librería especializada)
            return roots;
        }

        #endregion

        public override string ToString()
        {
            return $"H(s) = ({PolynomialToString(NumeratorCoefficients)}) / ({PolynomialToString(DenominatorCoefficients)})";
        }

        private string PolynomialToString(double[] coefficients)
        {
            if (coefficients.Length == 0) return "0";

            var terms = new List<string>();
            int order = coefficients.Length - 1;

            for (int i = 0; i < coefficients.Length; i++)
            {
                int power = order - i;
                double coeff = coefficients[i];

                if (Math.Abs(coeff) < 1e-10) continue;

                string term = "";
                if (power == 0)
                    term = $"{coeff:F2}";
                else if (power == 1)
                    term = $"{coeff:F2}s";
                else
                    term = $"{coeff:F2}s^{power}";

                terms.Add(term);
            }

            return terms.Count > 0 ? string.Join(" + ", terms) : "0";
        }
    }

    /// <summary>
    /// Respuesta en frecuencia
    /// </summary>
    public class FrequencyResponse
    {
        public List<double> Frequencies { get; set; }
        public List<double> Magnitude { get; set; }
        public List<double> Phase { get; set; }
    }
}