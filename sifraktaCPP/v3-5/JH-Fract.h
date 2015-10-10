/*
* JH-Fract (Header)
* Creates fractals with an algorithm by Julius Herb. The algorithm is based on the one from Waclaw Franciszek Sierpinski, but the count of corners is now variable.
* Works on Linux with gcc g++ and on Windows with MinGW g++. Other OS and compiler may work, but they were not tested yet.
* Licensed under the terms of WTFPL v2
* #BUGS: polygons with many corners have mistakes in the right lower corner!
* @author: Julius Herb (jgherb@live.de)
* @version: v3.5 (2015_10_10)
*/

#include <iostream>
#include <cstdlib>
#include <stdio.h>
#include <math.h> 
#include <cstring>
#include <sstream>
#include <algorithm>
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
bool ParamOutput();
void generateCorners();
