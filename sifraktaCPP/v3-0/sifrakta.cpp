/*
* JH-Fract
* Creates fractals with an algorithm by Julius Herb. The algorithm is based on the one from Waclaw Franciszek Sierpinski, but the count of corners is now variable.
* Works on Linux with gcc g++ and on Windows with MinGW g++. Other OS and compiler may work, but they were not tested yet.
* @author: Julius Herb (jgherb@live.de)
* @version: v3.0 (2015_10_05)
*/

#include <iostream>
#include <cstdlib>
#include <stdio.h>
#include <math.h> 
#include <cstring>
#include <sstream>
#include <algorithm>
#define PI 3.14159265

typedef struct {
    double r;       // percent
    double g;       // percent
    double b;       // percent
} rgb;

typedef struct {
    double h;       // angle in degrees
    double s;       // percent
    double v;       // percent
} hsv;
//Declarations
char* getCmdOption(char**, char**, const std::string&);
bool cmdOptionExists(char**, char**, const std::string&);
const char* Science(long);
int getposition(const char*, size_t, char);
long DeScience(const char*);
double Sin(double);
double Cos(double);
rgb int2RGB(int);

//Parameters
long report_freq = 30000000;
long iterations = 300000000;
int width = 1000;
int height = 1000;
int increment = 1;
int corner_count = 6;
int ColorMode = 0; //0=Multicolor; 1=Red; 2=Green; 3=Blue
int ColorMode_s = 50;
int ColorMode_v = 80;

//Array of corners
int corners[3][2]={{0,width/2},{height,0},{height,width}};

//Main method
int main(int argc, char* argv[]) {
    std::cout << "JH-Fract v3.0 ::: Julius Herb (jgherb@live.de) ::: use -help for Help\n";
    if(cmdOptionExists(argv, argv+argc, "-help"))
    {
        std::cout << "Argument Syntax:" << "\n";
        std::cout << "_____________________________________________________________________" << "\n";
        std::cout << "| Iterations:       | -i    |                                       |" << "\n";
        std::cout << "| Width:            | -w    |                                       |" << "\n";
        std::cout << "| Heigth:           | -h    |                                       |" << "\n";
        std::cout << "| Corner count:     | -c    |                                       |" << "\n";
        std::cout << "| ColorMode:        | -cm   | (0=MultiColor;1=Red;2=Green;3=Blue)   |" << "\n";
        std::cout << "| ColorMode_s:      | -cs   | (Saturation of HSV)                   |" << "\n";
        std::cout << "| ColorMode_v:      | -cv   | (Value of HSV)                        |" << "\n";
        std::cout << "| Increment:        | -ic   |                                       |" << "\n";
        std::cout << "| Report frequency: | -rf   |                                       |" << "\n";
        std::cout << "| Help:             | -help |                                       |" << "\n";
        std::cout << "_____________________________________________________________________" << "\n";
        std::cout << "\n";
        return 0;
    }    
    if(cmdOptionExists(argv, argv+argc, "-i"))
    {
        iterations = atol(getCmdOption(argv, argv + argc, "-i"));
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
        ColorMode_s = atoi(getCmdOption(argv, argv + argc, "-cs"));
    }
    if(cmdOptionExists(argv, argv+argc, "-cv"))
    {
        ColorMode_v = atoi(getCmdOption(argv, argv + argc, "-cv"));
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
    std::cout << "Settings:\n" << "-Iterations: " << Science(iterations) << "\n-Report steps: " << 100/faktor << "%\n-Resolution: " << width << "px X " << height << "px = " << mpixel << " MPixel\n-Corners: " << corner_count << "\n-Color increment: " << increment << "\n";
    if(ColorMode==0) {
        std::cout << "-ColorMode: " << "Multi color" << " (S: " << ColorMode_s << " V: " << ColorMode_v << ")" << "\n";
        badParameters = false;
    }
    if(ColorMode==1) {
        std::cout << "-ColorMode: " << "Red" << "\n";
        badParameters = false;
    }
    if(ColorMode==2) {
        std::cout << "-ColorMode: " << "Green" << "\n";
        badParameters = false;
    }
    if(ColorMode==3) {
        std::cout << "-ColorMode: " << "Blue" << "\n";
        badParameters = false;
    }
    if(badParameters) {
        std::cout << "BAD PARAMETERS!" << "\n";
        return -1;
    }
    std::cout << "Computation:\n";
    double daten[corner_count][2];
    corners[corner_count][2];
    double seite = 10;
    double rad = seite / (2 * Sin(180 / corner_count));
    double winkela = 360 / corner_count;//180-360/corner_count;
    double winkelc = 180 - 2 * winkela;
    for (int i = 0; i < corner_count; i++)
    {
        daten[i][1] = Cos(winkelc) * rad;
        daten[i][0] = Sin(winkelc) * rad;
        winkelc = winkelc + winkela;
    }

    double min = 0;
    for (int i = 0; i < corner_count; i++)
    {
        if (min > daten[i][0])
        {
            min = daten[i][0];
        }
    }
    double max = 0;
    for (int i = 0; i < corner_count; i++)
    {
        if (max < daten[i][1])
        {
            max = daten[i][1];
        }
    }
    for (int i = 0; i < corner_count; i++)
    {
        daten[i][0] = daten[i][0] - min;
        daten[i][1] = (daten[i][1] - max);
    }
    double max2 = 0;
    for (int i = 0; i < corner_count; i++)
    {
        if (max2 < daten[i][0])
        {
            max2 = daten[i][0];
        }
    }
    double max1 = 0;
    for (int i = 0; i < corner_count; i++)
    {
        if (max1 > daten[i][1])
        {
            max1 = daten[i][1];
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
        corners[i][0] = (int)(daten[i][0] * scale);
        corners[i][1] = (int)(daten[i][1] * scale * (-1));
    }
    int* sourcebuffer = new int[width*height];
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
            std::cout << status << "%\n";
        }
    }
    std::cout << "100%\n";
    std::cout << "Saving...\n";
    typedef struct                       /**** BMP file header structure ****/
        {
        unsigned int   bfSize;           /* Size of file */
        unsigned short bfReserved1;      /* Reserved */
        unsigned short bfReserved2;      /* ... */
        unsigned int   bfOffBits;        /* Offset to bitmap data */
        } BITMAPFILEHEADER;

    typedef struct                       /**** BMP file info structure ****/
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
    BITMAPFILEHEADER bfh;
    BITMAPINFOHEADER bih;
    
    /* Magic number for file. It does not fit in the header structure due to alignment  requirements, so put it outside */
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
    std::stringstream ss;
    #if __gnu_linux__
    ss << "" ;
    #else
    ss << "C:\\Users\\Public\\JH-Fract\\";
    #endif
    ss << "JH-Fract_c-" << corner_count << "_w-" << width << "_h-" << height << "_i-" << Science(iterations) << "_cm-" << ColorMode;
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
                rgb RGB = int2RGB(sourcebuffer[(y * width + x)]);
                r = RGB.r;
                g = RGB.g;
                b = RGB.b;
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
            fwrite(&b, 1, 1, file);
            fwrite(&g, 1, 1, file);
            fwrite(&r, 1, 1, file);
        }
    }
    fclose(file);
    std::cout << "DONE\n";
    return 0;
}
const char* Science(long value) {
    int digits = log10(value)+1;
    long div = pow(10,digits-2);
    std::stringstream ss;
    long val = value/div;
    long firstvalue = val / 10;
    long secondvalue = val % 10;
    digits--;
    ss << firstvalue << "." << secondvalue << "E" << digits;
    return ss.str().c_str();
}
int getposition(const char *array, size_t size, char c)
{
    const char* end = array + size;
    const char* match = std::find(array, end, c);
    return (end == match)? -1 : (match-array);
}
long DeScience(const char* input) {
    /*int index = getPosition(input,strlen(input),"E");
    std::string firststring = SubstringOfCString(input,0,index-1);
    long firstvalue = stol(firststring);
    std::string secondstring = SubstringOfCString(input,0,index-1);
    long firstvalue = stol(firststring);*/
    long value = 0;
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
static rgb   hsv2rgb(hsv in);
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
hsv rgb2hsv(rgb in)
{
    hsv         out;
    double      min, max, delta;

    min = in.r < in.g ? in.r : in.g;
    min = min  < in.b ? min  : in.b;

    max = in.r > in.g ? in.r : in.g;
    max = max  > in.b ? max  : in.b;

    out.v = max;                                // v
    delta = max - min;
    if (delta < 0.00001)
    {
        out.s = 0;
        out.h = 0; // undefined, maybe nan?
        return out;
    }
    if( max > 0.0 ) { // NOTE: if Max is == 0, this divide would cause a crash
        out.s = (delta / max);                  // s
    } else {
        // if max is 0, then r = g = b = 0              
            // s = 0, v is undefined
        out.s = 0.0;
        out.h = NAN;                            // its now undefined
        return out;
    }
    if( in.r >= max )                           // > is bogus, just keeps compilor happy
        out.h = ( in.g - in.b ) / delta;        // between yellow & magenta
    else
    if( in.g >= max )
        out.h = 2.0 + ( in.b - in.r ) / delta;  // between cyan & yellow
    else
        out.h = 4.0 + ( in.r - in.g ) / delta;  // between magenta & cyan

    out.h *= 60.0;                              // degrees

    if( out.h < 0.0 )
        out.h += 360.0;

    return out;
}


rgb hsv2rgb(hsv in)
{
    double      hh, p, q, t, ff;
    long        i;
    rgb         out;

    if(in.s <= 0.0) {       // < is bogus, just shuts up warnings
        out.r = in.v;
        out.g = in.v;
        out.b = in.v;
        return out;
    }
    hh = in.h;
    if(hh >= 360.0) hh = 0.0;
    hh /= 60.0;
    i = (long)hh;
    ff = hh - i;
    p = in.v * (1.0 - in.s);
    q = in.v * (1.0 - (in.s * ff));
    t = in.v * (1.0 - (in.s * (1.0 - ff)));

    switch(i) {
    case 0:
        out.r = in.v;
        out.g = t;
        out.b = p;
        break;
    case 1:
        out.r = q;
        out.g = in.v;
        out.b = p;
        break;
    case 2:
        out.r = p;
        out.g = in.v;
        out.b = t;
        break;

    case 3:
        out.r = p;
        out.g = q;
        out.b = in.v;
        break;
    case 4:
        out.r = t;
        out.g = p;
        out.b = in.v;
        break;
    case 5:
    default:
        out.r = in.v;
        out.g = p;
        out.b = q;
        break;
    }
    return out;     
}
char* getCmdOption(char ** begin, char ** end, const std::string & option)
{
    char ** itr = std::find(begin, end, option);
    if (itr != end && ++itr != end)
    {
        return *itr;
    }
    return 0;
}

bool cmdOptionExists(char** begin, char** end, const std::string& option)
{
    return std::find(begin, end, option) != end;
}
