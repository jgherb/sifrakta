/*
* JH-Fract
* Creates fractals with an algorithm by Julius Herb. The algorithm is based on the one from Waclaw Franciszek Sierpinski, but the count of corners is now variable.
* Works on Linux with gcc g++ and on Windows with MinGW g++. Other OS and compiler may work, but they were not tested yet.
* Licensed under the terms of WTFPL v2
* #BUGS: polygons with many corners have mistakes in the right lower corner!
* @author: Julius Herb (jgherb@live.de)
* @version: v4.0-dev (2015_10_09)
*/

#include "JH-Fract.h"
#include <iostream>
#include <cstdlib>
#include <stdio.h>
#include <math.h> 
#include <cstring>
#include <sstream>
#include <algorithm>
#include <vector>
using namespace std;
#define PI 3.14159265

//Parameters
///////////////////////////////////////////////////////////////////
double report_freq = DeScience("1e8"); //in percent
long iterations = DeScience("3e8");
int width = 2000;
int height = 2000;
int increment = 1;
int corner_count = 6;
int ColorMode = 0; //0=Multicolor; 1=Red; 2=Green; 3=Blue
double ColorMode_s = 1; //HSV-Saturation
double ColorMode_v = 1; //HSV-Value
int BG_R = 0;
int BG_G = 0;
int BG_B = 0;
void generateCorners();

//Arrays
vector<int> corners;
vector<int> sourcebuffer;

void generateCorners() {
    vector<double> daten (corner_count*2);
    corners.resize(corner_count*2);
    double seite = 10;
    double rad = seite / (2 * Sin(180 / corner_count));
    double winkela = 360 / corner_count;//180-360/corner_count;
    double winkelc = 180 - 2 * winkela;
    for (int i = 0; i < corner_count; i++)
    {
        daten[i*2+1] = Cos(winkelc) * rad;
        daten[i*2] = Sin(winkelc) * rad;
        winkelc = winkelc + winkela;
    }

    double min = 0;
    for (int i = 0; i < corner_count; i++)
    {
        if (min > daten[i*2])
        {
            min = daten[i*2];
        }
    }
    double max = 0;
    for (int i = 0; i < corner_count; i++)
    {
        if (max < daten[i*2+1])
        {
            max = daten[i*2+1];
        }
    }
    for (int i = 0; i < corner_count; i++)
    {
        daten[i*2] = daten[i*2] - min;
        daten[i*2+1] = (daten[i*2+1] - max);
    }
    double max2 = 0;
    for (int i = 0; i < corner_count; i++)
    {
        if (max2 < daten[i*20])
        {
            max2 = daten[i*2];
        }
    }
    double max1 = 0;
    for (int i = 0; i < corner_count; i++)
    {
        if (max1 > daten[i*2+1])
        {
            max1 = daten[i*2+1];
        }
    }
    max1 = (-1) * max1;
    double scale1 = height / max1;
    double scale2 = width / max2;
    double scale = scale1;
    if (scale1 < scale2)
    {
        scale = scale1;
    }
    if (scale2 < scale1)
    {
        scale = scale2;
    }
    int corners[corner_count][2];
    for (int i = 0; i < corner_count; i++)
    {
        corners[i*2] = (int)(daten[i*2] * scale);
        corners[i*2+1] = (int)(daten[i*2+1] * scale * (-1));
    }
}

//Main method
///////////////////////////////////////////////////////////////////
int main(int argc, char* argv[]) {
    cout << "JH-Fract v3.4 ::: Julius Herb (jgherb@live.de) ::: use -help for Help\n";
    if(cmdOptionExists(argv, argv+argc, "-help"))
    {
        cout << "Argument Syntax:" << "\n";
        cout << "_____________________________________________________________________" << "\n";
        cout << "| Iterations:       | -i    |                                       |" << "\n";
        cout << "| Width:            | -w    |                                       |" << "\n";
        cout << "| Heigth:           | -h    |                                       |" << "\n";
        cout << "| Corner count:     | -c    |                                       |" << "\n";
        cout << "| ColorMode:        | -cm   | (0=MultiColor;1=Red;2=Green;3=Blue)   |" << "\n";
        cout << "| ColorMode_s:      | -cs   | (Saturation of HSV)                   |" << "\n";
        cout << "| ColorMode_v:      | -cv   | (Value of HSV)                        |" << "\n";
        cout << "| Background r:     | -br   | (Red value of Background color)       |" << "\n";
        cout << "| Background g:     | -bg   | (Green value of Background color)     |" << "\n";
        cout << "| Background b:     | -bb   | (Blue value of Background color)      |" << "\n";
        cout << "| Increment:        | -ic   |                                       |" << "\n";
        cout << "| Report frequency: | -rf   | (in iterations)                       |" << "\n";
        cout << "| Help:             | -help |                                       |" << "\n";
        cout << "_____________________________________________________________________" << "\n";
        cout << "\n";
        cout << "License: " << "WTFPL v2" << "\n";
        return 0;
    }
    //TODO: Argument for extracting parameters from filename
    /*if(cmdOptionExists(argv, argv+argc, "-e"))
    {
        encodeFileName(getCmdOption(argv, argv + argc, "-e"));
    }   */
    if(cmdOptionExists(argv, argv+argc, "-br"))
    {
        BG_R = atoi(getCmdOption(argv, argv + argc, "-br"));
    }
    if(cmdOptionExists(argv, argv+argc, "-bg"))
    {
        BG_G = atoi(getCmdOption(argv, argv + argc, "-bg"));
    }
    if(cmdOptionExists(argv, argv+argc, "-bb"))
    {
        BG_B = atoi(getCmdOption(argv, argv + argc, "-bb"));
    } 
    if(cmdOptionExists(argv, argv+argc, "-i"))
    {
        iterations = DeScience(getCmdOption(argv, argv + argc, "-i"));
    }
    if(cmdOptionExists(argv, argv+argc, "-w"))
    {
        width = atoi(getCmdOption(argv, argv + argc, "-w"));
    }
    if(cmdOptionExists(argv, argv+argc, "-h"))
    {
        height = atoi(getCmdOption(argv, argv + argc, "-h"));
    }
    if(cmdOptionExists(argv, argv+argc, "-c"))
    {
        corner_count = atoi(getCmdOption(argv, argv + argc, "-c"));
    }
    if(cmdOptionExists(argv, argv+argc, "-cm"))
    {
        ColorMode = atoi(getCmdOption(argv, argv + argc, "-cm"));
    }
    if(cmdOptionExists(argv, argv+argc, "-cs"))
    {
        ColorMode_s = atof(getCmdOption(argv, argv + argc, "-cs"));
    }
    if(cmdOptionExists(argv, argv+argc, "-cv"))
    {
        ColorMode_v = atof(getCmdOption(argv, argv + argc, "-cv"));
    }
    if(cmdOptionExists(argv, argv+argc, "-ic"))
    {
        increment = atoi(getCmdOption(argv, argv + argc, "-ic"));
    }
    if(cmdOptionExists(argv, argv+argc, "-rf"))
    {
        report_freq = atoi(getCmdOption(argv, argv + argc, "-rf"));
    }
    long resolution = width*height;
    double kpixel = resolution / 1000;
    double mpixel = kpixel / 1000;
    double faktor = iterations / report_freq;
    bool badParameters = true;
    cout << "Settings:\n" << "-Iterations: " << Science(iterations) << "\n-Report steps: " << report_freq << " iterations ("<<100/faktor<<"%)\n-Resolution: " << width << "px X " << height << "px = " << mpixel << " MPixel\n-Corners: " << corner_count << "\n-Color increment: " << increment << "\n";
    if(ColorMode==0) {
        cout << "-ColorMode: " << "Multi color" << " (S: " << ColorMode_s << " V: " << ColorMode_v << ")" << "\n";
        badParameters = false;
    }
    if(ColorMode==1) {
        cout << "-ColorMode: " << "Red" << "\n";
        badParameters = false;
    }
    if(ColorMode==2) {
        cout << "-ColorMode: " << "Green" << "\n";
        badParameters = false;
    }
    if(ColorMode==3) {
        cout << "-ColorMode: " << "Blue" << "\n";
        badParameters = false;
    }
    if(badParameters) {
        cout << "BAD PARAMETERS!" << "\n";
        return -1;
    }
    cout << "Computation:\n";
    generateCorners();
    int dat[2] = {5,4};
    dat[0] = corners[0][0];
    dat[1] = corners[0][1];
    dat[1] = width / 2;
    long counter = 0;
    long counter2 = 0;
    for (long i = 0; i < iterations - 1; i++)
    {
        counter++;
        int pkt = 0;
        pkt = rand()%corner_count;
        int PA = corners[pkt][0];
        int PB = corners[pkt][1];
        dat[0] = (dat[0] + PA) / 2;
        dat[1] = (dat[1] + PB) / 2;
        int a = dat[1];
        int b = dat[0];
        if(ColorMode==0)
        {
            if (sourcebuffer[(a * width + b)] + increment < 360)
            {
                int middelware = (int)(sourcebuffer[(a * width + b)] + increment);
                sourcebuffer[(a * width + b)] = middelware;
            }
            else
            {
                sourcebuffer[(a * width + b)] = 360;
            }
        }
        else
        {
            if (sourcebuffer[(a * width + b)] + increment < 256)
            {
                int middelware = (int)(sourcebuffer[(a * width + b)] + increment);
                sourcebuffer[(a * width + b)] = middelware;
            }
        }
        if(counter==report_freq) {
            counter = 0;
            counter2++;
            double status = counter2/faktor*100;
            cout << status << "%\n";
        }
    }
    cout << "100%\n";
    cout << "Saving...\n";
    
    BITMAPFILEHEADER bfh;
    BITMAPINFOHEADER bih;
    
    /* Magic number for file. It does not fit in the header structure due to align, NULLment  requirements, so put it outside */
    unsigned short bfType=0x4d42;           
    bfh.bfReserved1 = 0;
    bfh.bfReserved2 = 0;
    bfh.bfSize = 2+sizeof(BITMAPFILEHEADER) + sizeof(BITMAPINFOHEADER)+width*height*3;
    bfh.bfOffBits = 0x36;
    
    bih.biSize = sizeof(BITMAPINFOHEADER);
    bih.biWidth = width;
    bih.biHeight = height;
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
    ss << "c" << corner_count << "_w" << width << "_h" << height << "_i" << Science(iterations) << "_cm" << ColorMode;
    if(ColorMode==0) {
        ss << "_s-" << ColorMode_s << "_v-" << ColorMode_v;
    }
    else {
    }
    ss << ".bmp"; 
    FILE *file = fopen(ss.str().c_str(), "wb");
    if (!file)
    {
        printf("Could not write file\n");
        return -1;
    }
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
            if(ColorMode==0)
            {
                int val = sourcebuffer[(y * width + x)];
                if(val!=0) {
                rgb RGB = int2RGB(val);
                r = RGB.r*255;
                g = RGB.g*255;
                b = RGB.b*255;}
            }
            if(ColorMode==1)
            {
                r = (unsigned char)sourcebuffer[(y * width + x)];
            }
            if(ColorMode==2)
            {
                g = (unsigned char)sourcebuffer[(y * width + x)];
            }
            if(ColorMode==3)
            {
                b = (unsigned char)sourcebuffer[(y * width + x)];
            }
            if(sourcebuffer[(y * width + x)]==0) {
                r = BG_R;
                g = BG_G;
                b = BG_B;
            }
            fwrite(&b, 1, 1, file);
            fwrite(&g, 1, 1, file);
            fwrite(&r, 1, 1, file);
        }
    }
    fclose(file);
    cout << "Saved as: " << ss.str() << "\n";
    cout << "DONE\n";
    return 0;
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
    if(value==0) {
        rgb RGB;
        return RGB;
    }
    value %= 360;
    hsv HSV;
    HSV.h = value;
    HSV.s = ColorMode_s;
    HSV.v = ColorMode_v;
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
