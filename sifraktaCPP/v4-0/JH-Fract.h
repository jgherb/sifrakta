/*
* JH-Fract (Header)
* Creates fractals with various algorithm; e.g. a one from Julius Herb, which is based on the one from Waclaw Franciszek Sierpinski, but the count of corners is now variable.
* Works on Linux with gcc g++ and on Windows with MinGW g++. Other OS and compiler may work, but they were not tested yet. C++11 conform. Requires header file "JH-Fract.h". 
* Licensed under the terms of WTFPL v2
* #BUGS: polygons with many corners have mistakes in the right lower corner!
* @author: Julius Herb (jgherb@live.de)
* @version: v4.0 (2015_12_15)
*/

#include <iostream>
#include <cstdlib>
#include <stdio.h>
#include <math.h> 
#include <cstring>
#include <sstream>
#include <algorithm>
#include <complex>
#define PI 3.14159265

//Structs
///////////////////////////////////////////////////////////////////
//RGB Color space
typedef struct
{
    double r;       // percent
    double g;       // percent
    double b;       // percent
} rgb;
//HSL Color space
typedef struct
{
    double h;       // angle in degrees
    double s;       // percent
    double v;       // percent
} hsv;
//BMP file header structure
typedef struct
{
    unsigned int   bfSize;           /* Size of file */
    unsigned short bfReserved1;      /* Reserved */
    unsigned short bfReserved2;      /* ... */
    unsigned int   bfOffBits;        /* Offset to bitmap data */
} BITMAPFILEHEADER;
//BMP file info structure
typedef struct                       
{
    unsigned int   biSize;           /* Size of info header */
    int            biWidth;          /* Width of image */
    int            biHeight;         /* Height of image */
    unsigned short biPlanes;         /* Number of color planes */
    unsigned short biBitCount;       /* Number of bits per pixel */
    unsigned int   biCompression;    /* Type of compression to use */
    unsigned int   biSizeImage;      /* Size of image data */
    int            biXPelsPerMeter;  /* X pixels per meter */
    int            biYPelsPerMeter;  /* Y pixels per meter */
    unsigned int   biClrUsed;        /* Number of colors used */
    unsigned int   biClrImportant;   /* Number of important colors */
} BITMAPINFOHEADER;
typedef struct                       
{
    double cxmin = -2.5;
    double cymin = -1.0;
    double cxmax = 1.0;
    double cymax = 1.0;
    int max_iterations = 100;
    int report_freq = 100000000;
} MandelbrotParam;
//Declarations
///////////////////////////////////////////////////////////////////
char* getCmdOption(char**, char**, const std::string&);
bool cmdOptionExists(char**, char**, const std::string&);
const char* Science(long);
long Science(const char*);
int getposition(const char*, size_t, char);
double DeScience(const char*);
double Sin(double);
double Cos(double);
rgb int2RGB(int);
rgb hsv2rgb(hsv);
void encodeFileName(const char*);
void ParseArgs();
void sierpinski();
void feigenbaum();
void Mandelbrot(MandelbrotParam);
void Buddhabrot();
bool ParamOutput();
void generateCorners();
