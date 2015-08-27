using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Exemplo retirado de:  http://www.dyadica.co.uk/very-simple-kalman-in-c/
// Para o meu tipo de dados este filtro não é muito bom...
namespace SensorSomSerialCSharp
{
    class SimpleKalman
    {
        private static double Q = 0.000001;
        private static double R = 0.01;
        private static double P = 1, X = 0, K;

        private static void measurementUpdate()
        {
            K = (P + Q) / (P + Q + R);
            P = R * (P + Q) / (R + P + Q);
        }

        public static double update(double measurement)
        {
            measurementUpdate();
            double result = X + (measurement - X) * K;
            X = result;
            return result;
        }
    
    }
}
