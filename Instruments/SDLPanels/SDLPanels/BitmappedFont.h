#pragma once

#include <SDL.h>

class BitmappedFont
{
public:
	BitmappedFont();
	~BitmappedFont();
	
	void Load(const char * image, int width, int height);
	void DrawString(const char * str, int x, int y, bool centred);

private:
	SDL_Texture * tex;
	int w, h;
	int chars_per_line;
};