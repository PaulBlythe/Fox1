#include "PackedFont.h"
#include "SDL.h"

extern SDL_Renderer* gRenderer;

PackedFont::PackedFont()
{
}


PackedFont::~PackedFont()
{
}

void PackedFont::Load(const char * image, const char * descriptor)
{
	SDL_RWops* file = SDL_RWFromFile(descriptor, "r+b");
	Sint32 count;
	SDL_RWread(file, &count, sizeof(Sint32), 1);
	for (int i = 0; i < count; i++)
	{
		Sint32 id, x, y, w, h, xo, yo, xa;
		SDL_RWread(file, &id, sizeof(Sint32), 1);
		SDL_RWread(file, &x, sizeof(Sint32), 1);
		SDL_RWread(file, &y, sizeof(Sint32), 1);
		SDL_RWread(file, &w, sizeof(Sint32), 1);
		SDL_RWread(file, &h, sizeof(Sint32), 1);
		SDL_RWread(file, &xo, sizeof(Sint32), 1);
		SDL_RWread(file, &yo, sizeof(Sint32), 1);
		SDL_RWread(file, &xa, sizeof(Sint32), 1);

		records[id].ID = id;
		records[id].X = x;
		records[id].Y = y;
		records[id].Width = w;
		records[id].Height = h;
		records[id].XOffset = xo;
		records[id].YOffset = yo;
		records[id].XAdvance = xa;
	}
	SDL_RWclose(file);

	SDL_Surface * temp = SDL_LoadBMP(image);
	SDL_SetColorKey(temp, SDL_TRUE, SDL_MapRGB(temp->format, 0xFF, 0x00, 0xFF));
	font = SDL_CreateTextureFromSurface(gRenderer, temp);
	SDL_FreeSurface(temp);
}

void PackedFont::DrawString(const char * str, int x, int y, bool centred)
{
	int dx = x;
	if (centred)
	{
		int sz = StringWidth(str);
		dx -= sz / 2;
	}
	int i = 0;
	while (str[i] != 0)
	{
		int id = (int)str[i];
		id &= 255;
		SDL_Rect src = { records[id].X,records[id].Y,records[id].Width,records[id].Height };
		SDL_Rect dest = { 0,0,records[id].Width,records[id].Height };
		dest.x = dx + records[id].XOffset;
		dest.y = y + records[id].YOffset;

		SDL_RenderCopy(gRenderer, font, &src, &dest);
		dx += records[id].XAdvance;
		i++;
	}
}

int PackedFont::StringWidth(const char * str)
{
	int dx = 0;
	int i = 0;
	while (str[i] != 0)
	{
		int id = (int)str[i];
		id &= 255;
		
		dx += records[id].XAdvance;
		i++;
	}
	return dx;
}

void PackedFont::setColor(Uint8 red, Uint8 green, Uint8 blue)
{
	SDL_SetTextureColorMod(font, red, green, blue);

}
