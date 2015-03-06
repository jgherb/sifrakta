internal byte[] Draw(int width, int height, int vt)
{
    vtiefe = vt;
    int[,] Punkt3 = new int[3, 2];
    Punkt3[0, 0] = 0;
    Punkt3[0, 1] = height / 2;
    Punkt3[1, 0] = (int)Math.Sqrt((height * height) * 3 / 4);
    Punkt3[1, 1] = 0;
    Punkt3[2, 0] = (int)Math.Sqrt((height * height) * 3 / 4);
    Punkt3[2, 1] = height;
        // 4 bytes (RGBA) required for each pixel
    byte[] result = new byte[width * height * 4];
    int resultIndex = 0;
        // Max iterations when testing whether a point is in the set
    int maxIterationCount = 50;
        // Initialisierungen / Deklarationen
    Random zufall = new Random();
        // Plot the Mandelbrot set on x-y plane
        //Zufallszahl für verwendeten Eckpunkt wird generiert
    int[] dat = new int[2];
    for (int i = 0; i < vtiefe - 1; i++)
    {
      int pkt = zufall.Next(3);
         //Koordinaten werden bestimmt (Durchschnitt aus altem Punkt und Eckpunkt)
      dat[0] = (dat[0] + Punkt3[pkt, 0]) / 2;
      dat[1] = (dat[1] + Punkt3[pkt, 1]) / 2;
          // Shade pixel based on probability it's in the set
      result[(dat[0] * width + dat[1]) * 4 + 1] = (byte)(result[(dat[0] * width + dat[1]) * 4+1] +20);// Green value of pixel
    }
    return result;
  }
