/*
* JH-Fract
* Creates fractals with various algorithm; e.g. a one from Julius Herb, which is based on the one from Waclaw Franciszek Sierpinski, but the count of corners is now variable.
* Works on Linux with gcc g++ and on Windows with MinGW g++. Other OS and compiler may work, but they were not tested yet. C++11 conform. Requires header file "JH-Fract.h". 
* Licensed under the terms of WTFPL v2
* #BUGS: polygons with many corners have mistakes in the right lower corner!
* @author: Julius Herb (jgherb@live.de)
* @version: v4.1 (2015_12_16)
*/

#include "JH-Fract.h"
#include <iostream>
#include <cstdlib>
#include <stdio.h>
#include <math.h>
#include <cstring>
#include <sstream>
#include <fstream>
#include <algorithm>
#include <vector>
#include <complex>
using namespace std;

//Variablen
vector<int> corners;
vector<int> sourcebuffer;
double faktor = 1;
GeneralParam gp;
SierpinskiParam spp;
FeigenbaumParam fbp;
MandelbrotParam mbp;

//Main method
///////////////////////////////////////////////////////////////////
int main(int argc, char* argv[]) {
    cout << "JH-Fract v4.1 ::: Julius Herb (jgherb@live.de) | NOSCIO (noscio.ml) ::: use -help for Help\n";
    if(cmdOptionExists(argv, argv+argc, "-help"))
    {
        cout << "Argument Syntax:" << "\n";
        cout << "___________________________________________________________________________" << "\n";
        cout << "| Iterations:       | -i    |                                             |" << "\n";
        cout << "| Width:            | -w    |                                             |" << "\n";
        cout << "| Heigth:           | -h    |                                             |" << "\n";
        cout << "| Corner count:     | -c    |                                             |" << "\n";
        cout << "| gp.ColorMode:        | -cm   | (0=MultiColor;1=Red;2=Green;3=Blue;4=White) |" << "\n";
        cout << "| gp.ColorMode_s:      | -cs   | (Saturation of HSV)                         |" << "\n";
        cout << "| gp.ColorMode_v:      | -cv   | (Value of HSV)                              |" << "\n";
        cout << "| Background r:     | -br   | (Red value of Background color)             |" << "\n";
        cout << "| Background g:     | -bg   | (Green value of Background color)           |" << "\n";
        cout << "| Background b:     | -bb   | (Blue value of Background color)            |" << "\n";
        cout << "| Increment:        | -ic   |                                             |" << "\n";
        cout << "| Report frequency: | -rf   | (in iterations)                             |" << "\n";
        cout << "| Help:             | -help |                                             |" << "\n";
        cout << "___________________________________________________________________________" << "\n";
        cout << "\n";
        cout << "License: " << "WTFPL v2" << "\n";
        return 0;
    }
    //TODO: Argument for extracting parameters from filename
    /*if(cmdOptionExists(argv, argv+argc, "-e"))
    {
        encodeFileName(getCmdOption(argv, argv + argc, "-e"));
    }   */
    if(cmdOptionExists(argv, argv+argc, "-sp"))
    {
        gp.fractal_mode = 0;
    }
    if(cmdOptionExists(argv, argv+argc, "-fb"))
    {
        gp.fractal_mode = 1;
    }
    if(cmdOptionExists(argv, argv+argc, "-mb"))
    {
        gp.fractal_mode = 2;
    }
    switch(gp.fractal_mode) {
        case 0:
            if(cmdOptionExists(argv, argv+argc, "-i"))
            {
                spp.iterations = DeScience(getCmdOption(argv, argv + argc, "-i"));
            }
            if(cmdOptionExists(argv, argv+argc, "-c"))
            {
                spp.corner_count = atoi(getCmdOption(argv, argv + argc, "-c"));
            }
            if(cmdOptionExists(argv, argv+argc, "-ic"))
            {
                spp.increment = atoi(getCmdOption(argv, argv + argc, "-ic"));
            }
            if(cmdOptionExists(argv, argv+argc, "-rf"))
            {
                spp.report_freq = DeScience(getCmdOption(argv, argv + argc, "-rf"));
            }
            break;
        case 1:
            if(cmdOptionExists(argv, argv+argc, "-i"))
            {
                fbp.iterations = DeScience(getCmdOption(argv, argv + argc, "-i"));
            }
            if(cmdOptionExists(argv, argv+argc, "-ic"))
            {
                fbp.increment = atoi(getCmdOption(argv, argv + argc, "-ic"));
            }
            if(cmdOptionExists(argv, argv+argc, "-rf"))
            {
                fbp.report_freq = DeScience(getCmdOption(argv, argv + argc, "-rf"));
            }
            break;
        case 2:
            if(cmdOptionExists(argv, argv+argc, "-i"))
            {
                mbp.max_iterations = DeScience(getCmdOption(argv, argv + argc, "-i"));
            }
            if(cmdOptionExists(argv, argv+argc, "-ic"))
            {
                mbp.increment = atoi(getCmdOption(argv, argv + argc, "-ic"));
            }
            if(cmdOptionExists(argv, argv+argc, "-rf"))
            {
                mbp.report_freq = DeScience(getCmdOption(argv, argv + argc, "-rf"));
            }
            break;
    }
    if(cmdOptionExists(argv, argv+argc, "-br"))
    {
        gp.BG_R = atoi(getCmdOption(argv, argv + argc, "-br"));
    }
    if(cmdOptionExists(argv, argv+argc, "-bg"))
    {
        gp.BG_G = atoi(getCmdOption(argv, argv + argc, "-bg"));
    }
    if(cmdOptionExists(argv, argv+argc, "-bb"))
    {
        gp.BG_B = atoi(getCmdOption(argv, argv + argc, "-bb"));
    }
    if(cmdOptionExists(argv, argv+argc, "-w"))
    {
        gp.width = atoi(getCmdOption(argv, argv + argc, "-w"));
    }
    if(cmdOptionExists(argv, argv+argc, "-h"))
    {
        gp.height = atoi(getCmdOption(argv, argv + argc, "-h"));
    }
    if(cmdOptionExists(argv, argv+argc, "-cm"))
    {
        gp.ColorMode = atoi(getCmdOption(argv, argv + argc, "-cm"));
    }
    if(cmdOptionExists(argv, argv+argc, "-cs"))
    {
        gp.ColorMode_s = atof(getCmdOption(argv, argv + argc, "-cs"));
    }
    if(cmdOptionExists(argv, argv+argc, "-cv"))
    {
        gp.ColorMode_v = atof(getCmdOption(argv, argv + argc, "-cv"));
    }
    if(ParamOutput()) {
        //return -1;
    }
    cout << "Computation:\n";
    switch(gp.fractal_mode) {
        case 0:
            generateCorners();
            sierpinski(spp);
            break;
        case 1:
            feigenbaum(fbp);
            break;
        case 2:
            Mandelbrot(mbp);
            break;
    }
    cout << "Saving...\n";
    
    BITMAPFILEHEADER bfh;
    BITMAPINFOHEADER bih;
    
    /* Magic number for file. It does not fit in the header structure due to align, NULLment  requirements, so put it outside */
    unsigned short bfType=0x4d42;           
    bfh.bfReserved1 = 0;
    bfh.bfReserved2 = 0;
    bfh.bfSize = 2+sizeof(BITMAPFILEHEADER) + sizeof(BITMAPINFOHEADER)+gp.width*gp.height*3;
    bfh.bfOffBits = 0x36;
    
    bih.biSize = sizeof(BITMAPINFOHEADER);
    bih.biWidth = gp.width;
    bih.biHeight = gp.height;
    bih.biPlanes = 1;
    bih.biBitCount = 24;
    bih.biCompression = 0;
    bih.biSizeImage = 0;
    bih.biXPelsPerMeter = 5000;
    bih.biYPelsPerMeter = 5000;
    bih.biClrUsed = 0;
    bih.biClrImportant = 0;
    stringstream ss;
    #if __gnu_linux__
    ss << "" ;
    #else
    ss << "C:\\Users\\Public\\JH-Fract\\";
    #endif
    /*ss << "c" << corner_count << "_w" << gp.width << "_h" << gp.height << "_i" << Science(iterations) << "_cm" << gp.ColorMode;
    if(gp.ColorMode==0) {
        ss << "_s-" << gp.ColorMode_s << "_v-" << gp.ColorMode_v;
    }
    else {
    }*/
    ss << ".bmp";
    time_t timer;
    time(&timer);
    stringstream ts;
    ts << "fract" << timer << ".bmp";
    stringstream ts2;
    ts2 << "fract" << timer << ".txt";
    FILE *file = fopen(ts.str().c_str(), "wb");
    if (!file)
    {
        printf("Could not write file\n");
        return -1;
    }
    /*ofstream myFile;
    myFile.open(ts2.str().c_str(),ofstream::out);
    myFile << ss.str() << "\n";
    long resolution = gp.width*gp.height;
    double kpixel = resolution / 1000;
    double mpixel = kpixel / 1000;
    faktor = iterations / report_freq;
    bool badParameters = true;
    myFile << "Fractal: ";
    switch(fractal_mode) {
        case 0:
            myFile << "Sierpinski";
            break;
        case 1:
            myFile << "Feigenbaum";
            break;
        case 2:
            myFile << "Mandelbrot";
            break;
    }
    myFile << "\n";
    myFile << "Settings:\n" << "-Iterations: " << Science(iterations) << "\n-Report steps: " << report_freq << " iterations ("<<100/faktor<<"%)\n-Resolution: " << gp.width << "px X " << gp.height << "px = " << mpixel << " MPixel\n-Corners: " << corner_count << "\n-Color increment: " << increment << "\n";
    if(gp.ColorMode==0) {
        myFile << "-gp.ColorMode: " << "Multi color" << " (S: " << gp.ColorMode_s << " V: " << gp.ColorMode_v << ")" << "\n";
        badParameters = false;
    }
    if(gp.ColorMode==1) {
        myFile << "-gp.ColorMode: " << "Red" << "\n";
        badParameters = false;
    }
    if(gp.ColorMode==2) {
        myFile << "-gp.ColorMode: " << "Green" << "\n";
        badParameters = false;
    }
    if(gp.ColorMode==3) {
        myFile << "-gp.ColorMode: " << "Blue" << "\n";
        badParameters = false;
    }
    if(gp.ColorMode==4) {
        myFile << "-gp.ColorMode: " << "White" << "\n";
        badParameters = false;
    }
    if(badParameters) {
        myFile << "BAD PARAMETERS!" << "\n";
        return true;
    }
    myFile << "\n--------------------------------------------------------------\n"
             << "JH-Fract ::: Julius Herb (jgherb@live.de) | NOSCIO (noscio.ml)";
    myFile.close();*/
    /*Write headers*/
    fwrite(&bfType,1,sizeof(bfType),file);
    fwrite(&bfh, 1, sizeof(bfh), file);
    fwrite(&bih, 1, sizeof(bih), file);
    /*Write bitmap*/
    for (int y = bih.biHeight-1; y>=0; y--) /*Scanline loop backwards*/
    {
        for (int x = 0; x < bih.biWidth; x++) /*Column loop forwards*/
        {
            unsigned char r = 0;
            unsigned char g = 0;
            unsigned char b = 0;
            if(gp.ColorMode==0)
            {
                int val = sourcebuffer[(y * gp.width + x)];
                if(val!=0) {
                rgb RGB = int2RGB(val);
                r = RGB.r*255;
                g = RGB.g*255;
                b = RGB.b*255;}
            }
            if(gp.ColorMode==1)
            {
                r = (unsigned char)sourcebuffer[(y * gp.width + x)];
            }
            if(gp.ColorMode==2)
            {
                g = (unsigned char)sourcebuffer[(y * gp.width + x)];
            }
            if(gp.ColorMode==3)
            {
                b = (unsigned char)sourcebuffer[(y * gp.width + x)];
            }
            if(gp.ColorMode==4) {
                r = b = g = (unsigned char)sourcebuffer[(y * gp.width + x)];
            }
            if(sourcebuffer[(y * gp.width + x)]==0) {
                r = gp.BG_R;
                g = gp.BG_G;
                b = gp.BG_B;
            }
            fwrite(&b, 1, 1, file);
            fwrite(&g, 1, 1, file);
            fwrite(&r, 1, 1, file);
        }
    }
    fclose(file);
    cout << "Saved as: " << ts.str() << "\n";
    cout << "DONE\n";
    return 0;
}
bool ParamOutput() {
    long resolution = gp.width*gp.height;
    double kpixel = resolution / 1000;
    double mpixel = kpixel / 1000;
    bool badParameters = true;
    cout << "Fractal: ";
    switch(gp.fractal_mode) {
        case 0:
            cout << "Sierpinski";
            break;
        case 1:
            cout << "Feigenbaum";
            break;
        case 2:
            cout << "Mandelbrot";
            break;
    }
    cout << "\n";    
    /*long resolution = gp.width*gp.height;
    double kpixel = resolution / 1000;
    double mpixel = kpixel / 1000;
    faktor = iterations / report_freq;
    bool badParameters = true;
    cout << "Fractal: ";
    switch(gp.fractal_mode) {
        case 0:
            cout << "Sierpinski";
            break;
        case 1:
            cout << "Feigenbaum";
            break;
        case 2:
            cout << "Mandelbrot";
            break;
    }
    cout << "\n";
    cout << "Settings:\n" << "-Iterations: " << Science(iterations) << "\n-Report steps: " << report_freq << " iterations ("<<100/faktor<<"%)\n-Resolution: " << gp.width << "px X " << gp.height << "px = " << mpixel << " MPixel\n-Corners: " << corner_count << "\n-Color increment: " << increment << "\n";
    if(gp.ColorMode==0) {
        cout << "-gp.ColorMode: " << "Multi color" << " (S: " << gp.ColorMode_s << " V: " << gp.ColorMode_v << ")" << "\n";
        badParameters = false;
    }
    if(gp.ColorMode==1) {
        cout << "-gp.ColorMode: " << "Red" << "\n";
        badParameters = false;
    }
    if(gp.ColorMode==2) {
        cout << "-gp.ColorMode: " << "Green" << "\n";
        badParameters = false;
    }
    if(gp.ColorMode==3) {
        cout << "-gp.ColorMode: " << "Blue" << "\n";
        badParameters = false;
    }
    if(gp.ColorMode==4) {
        cout << "-gp.ColorMode: " << "White" << "\n";
        badParameters = false;
    }
    if(badParameters) {
        cout << "BAD PARAMETERS!" << "\n";
        return true;
    }*/
}

void sierpinski(SierpinskiParam spp)
{
    double faktor = spp.iterations/spp.report_freq;
    int dat[2] = {5,4};
    dat[0] = corners[0];
    dat[1] = corners[1];
    dat[1] = gp.width / 2;
    long counter = 0;
    long counter2 = 0;
    sourcebuffer.reserve(gp.width*gp.height);
    for (long i = 0; i < spp.iterations - 1; i++)
    {
        counter++;
        int pkt = 0;
        pkt = rand()%spp.corner_count;
        int PA = corners[pkt*2];
        int PB = corners[pkt*2+1];
        dat[0] = (dat[0] + PA) / 2;
        dat[1] = (dat[1] + PB) / 2;
        int a = dat[1];
        int b = dat[0];
        if(gp.ColorMode==0)
        {
            //cout << i << "a\n";
            if (sourcebuffer[(a * gp.width + b)] + spp.increment < 360)
            {
                //cout << i << "b\n";
                int middelware = (int)(sourcebuffer[(a * gp.width + b)] + spp.increment);
                sourcebuffer[(a * gp.width + b)] = middelware;
            }
            else
            {
                sourcebuffer[(a * gp.width + b)] = 360;
            }
        }
        else
        {
            if (sourcebuffer[(a * gp.width + b)] + spp.increment < 256)
            {
                int middelware = (int)(sourcebuffer[(a * gp.width + b)] + spp.increment);
                sourcebuffer[(a * gp.width + b)] = middelware;
            }
        }
        //cout << i << "c\n";
        if(counter==spp.report_freq) {
            counter = 0;
            counter2++;
            double status = counter2/faktor*100;
            cout << status << "%\n";
        }
    }
    cout << "100%\n";
}

void feigenbaum(FeigenbaumParam param)
{
    long iterations = param.iterations;
    double start = param.start;
    double x1 = param.x1;
    double x2 = param.x2;
    double y1 = param.y1;
    double y2 = param.y2;
    sourcebuffer.reserve(gp.width*gp.height);
    double seq = (x2 - x1) / gp.width;
    double ergebnis = start;
    int zaehler = -1;
    for (double p = x1; p < x2; p = p + seq)
    {
        //cout << "P:" << p << "\n";
        zaehler = zaehler + 1;
        ergebnis = start;
        for (int i = 0; i < iterations; i++)
        {
            //neues Folgeglied der logistischen Gleichung wird berechnet
            ergebnis = p * ergebnis * (1 - ergebnis);
            //Höhe im Ausgabebild wird berechnet
            int h = gp.height - (int)(ergebnis * (gp.height - 1) / (y2 - y1) - (gp.height - 1) / (y2 - y1) * y1);
            if (zaehler < gp.width & h >= 0 & h < gp.height)
            {
                if (sourcebuffer[(zaehler + h * gp.width)] + param.increment < 256)
                {
                    //cout << "C:" << i << "\n";
                    int middelwert = (int)(sourcebuffer[(zaehler + h * gp.width)] + param.increment);
                    //cout << "D:" << i << "\n";
                    sourcebuffer[(zaehler + h * gp.width)] = middelwert;
                }
                else
                {
                    sourcebuffer[zaehler + h * gp.width] = 0;
                }
            }
        }
    }
    cout << "100%\n";
}

void Mandelbrot(MandelbrotParam param) 
{
    int max_iterations = param.max_iterations;
    sourcebuffer.reserve(gp.width*gp.height);
    double cxmin = param.cxmin;
    double cymin = param.cymin;
    double cxmax = param.cxmax;
    double cymax = param.cymax;
    for (int iy = 0; iy < gp.height; iy++)
    {
        for (int ix = 0; ix < gp.width; ix++)
        {
            std::complex<double> c(cxmin + ix/(gp.width-1.0)*(cxmax-cxmin), cymin + iy/(gp.height-1.0)*(cymax-cymin));
            std::complex<double> z = 0;
            unsigned int iterations;
            for (iterations = 0; iterations < max_iterations && std::abs(z) < 2.0; ++iterations)
            {
                z = z*z + c;
            }
            double value = iterations * 360 / max_iterations;
            sourcebuffer[iy*gp.width+ix] = value;//(iterations == max_iterations) ? 255 : 0;
        }
    }
}

void Buddhabrot() {
    

}

void generateCorners()
{
    vector<double> daten (spp.corner_count*2);
    corners.resize(spp.corner_count*2);
    double seite = 10;
    double rad = seite / (2 * Sin(180 / spp.corner_count));
    double winkela = 360 / spp.corner_count;//180-360/corner_count;
    double winkelc = 180 - 2 * winkela;
    for (int i = 0; i < spp.corner_count; i++)
    {
        daten[i*2+1] = Cos(winkelc) * rad;
        daten[i*2] = Sin(winkelc) * rad;
        winkelc = winkelc + winkela;
    }

    double min = 0;
    for (int i = 0; i < spp.corner_count; i++)
    {
        if (min > daten[i*2])
        {
            min = daten[i*2];
        }
    }
    double max = 0;
    for (int i = 0; i < spp.corner_count; i++)
    {
        if (max < daten[i*2+1])
        {
            max = daten[i*2+1];
        }
    }
    for (int i = 0; i < spp.corner_count; i++)
    {
        daten[i*2] = daten[i*2] - min;
        daten[i*2+1] = (daten[i*2+1] - max);
    }
    double max2 = 0;
    for (int i = 0; i < spp.corner_count; i++)
    {
        if (max2 < daten[i*20])
        {
            max2 = daten[i*2];
        }
    }
    double max1 = 0;
    for (int i = 0; i < spp.corner_count; i++)
    {
        if (max1 > daten[i*2+1])
        {
            max1 = daten[i*2+1];
        }
    }
    max1 = (-1) * max1;
    double scale1 = gp.height / max1;
    double scale2 = gp.width / max2;
    double scale = scale1;
    if (scale1 < scale2)
    {
        scale = scale1;
    }
    if (scale2 < scale1)
    {
        scale = scale2;
    }
    for (int i = 0; i < spp.corner_count; i++)
    {
        corners[i*2] = (int)(daten[i*2] * scale);
        corners[i*2+1] = (int)(daten[i*2+1] * scale * (-1));
    }
}
const char* Science(long value) {
    int digits = log10(value)+1;
    long div = pow(10,digits-2);
    stringstream ss;
    long val = value/div;
    long firstvalue = val / 10;
    long secondvalue = val % 10;
    digits--;
    ss << firstvalue << "." << secondvalue << "e+" << digits;
    return ss.str().c_str();
}
int getposition(const char *array, size_t size, char c)
{
    const char* end = array + size;
    const char* match = find(array, end, c);
    return (end == match)? -1 : (match-array);
}
double DeScience(const char* input) {
    stringstream ss;
    ss << input;
    int index = ss.str().find('e');
    string firststring = ss.str().substr(0,index);
    double firstvalue = strtod(firststring.c_str(), NULL);
    string secondstring = ss.str().substr(index+1);
    double secondvalue = strtod(secondstring.c_str(), NULL);
    double value = firstvalue*pow(10,secondvalue);
    return value;
}
double Sin(double d)
{
    return sin(d * PI / 180);
}
double Cos(double d)
{
    return cos(d * PI / 180);
}


static hsv   rgb2hsv(rgb in);

rgb int2RGB(int value) {
    if(value==0)
    {
        rgb RGB;
        return RGB;
    }
    if(value>360)
    {
        value = 360;
    }
    hsv HSV;
    HSV.h = value;
    HSV.s = gp.ColorMode_s;
    HSV.v = gp.ColorMode_v;
    return hsv2rgb(HSV);
}

rgb hsv2rgb(hsv in)
{
    double h = in.h;
    double s = in.s;
    double v = in.v;
    rgb RGB;
    int i;
	double f, p, q, t;

	if( s == 0 ) {
		// achromatic (grey)
		RGB.r = RGB.g = RGB.b = v;
		return RGB;
	}

	h /= 60;			// sector 0 to 5
	i = floor( h );
	f = h - i;			// factorial part of h
	p = v * ( 1 - s );
	q = v * ( 1 - s * f );
	t = v * ( 1 - s * ( 1 - f ) );

	switch( i ) {
		case 0:
			RGB.r = v;
			RGB.g = t;
			RGB.b = p;
			break;
		case 1:
			RGB.r = q;
			RGB.g = v;
			RGB.b = p;
			break;
		case 2:
			RGB.r = p;
			RGB.g = v;
			RGB.b = t;
			break;
		case 3:
			RGB.r = p;
			RGB.g = q;
			RGB.b = v;
			break;
		case 4:
			RGB.r = t;
			RGB.g = p;
			RGB.b = v;
			break;
		default:		// case 5:
			RGB.r = v;
			RGB.g = p;
			RGB.b = q;
			break;
	}
    return RGB;     
}


char* getCmdOption(char ** begin, char ** end, const string & option)
{
    char ** itr = find(begin, end, option);
    if (itr != end && ++itr != end)
    {
        return *itr;
    }
    return 0;
}

void encodeFileName(const char * input)
{
    stringstream ss;
    ss << input;
    int index = ss.str().find('e');
    string firststring = ss.str().substr(0,index);
    double firstvalue = strtod(firststring.c_str(), NULL);
    string secondstring = ss.str().substr(index+1);
    double secondvalue = strtod(secondstring.c_str(), NULL);
    double value = firstvalue*pow(10,secondvalue);
}

bool cmdOptionExists(char** begin, char** end, const string& option)
{
    return find(begin, end, option) != end;
}
