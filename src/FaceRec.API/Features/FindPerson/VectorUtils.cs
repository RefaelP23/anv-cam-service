using System;

namespace FaceRec.API.Features.FindPerson
{
    public static class VectorUtils
    { 
        public static double CalculateVectorCosine(double[] v1, double[] v2)
        {
            var length = (v2.Length < v1.Length) ? v2.Length : v1.Length;
            double dot = 0.0d;
            double mag1 = 0.0d;
            double mag2 = 0.0d;
            for (int n = 0; n < length; n++)
            {
                dot += v1[n] * v2[n];
                mag1 += Math.Pow(v1[n], 2);
                mag2 += Math.Pow(v2[n], 2);
            }

            return dot / (Math.Sqrt(mag1) * Math.Sqrt(mag2));
        }
    }
}
