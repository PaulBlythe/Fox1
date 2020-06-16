#pragma once
#include "SDL.h"

struct PackedFontRecord
{
	int ID;
	int X;
	int Y;
	int Width;
	int Height;
	int XOffset;
	int YOffset;
	int XAdvance;
};

class PackedFont
{
public:
	PackedFont();
	~PackedFont();

	void Load(const char * image, const char * descriptor);
	void DrawString(const char * str, int x, int y, bool centred);
	int StringWidth(const char * str);
	void setColor(Uint8 red, Uint8 green, Uint8 blue);


private:
	struct PackedFontRecord records[256];
	SDL_Texture* font;
};

