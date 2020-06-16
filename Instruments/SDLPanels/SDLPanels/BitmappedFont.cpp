#include "BitmappedFont.h"

extern SDL_Renderer* gRenderer;

BitmappedFont::BitmappedFont()
{
}

BitmappedFont::~BitmappedFont()
{
	SDL_DestroyTexture(tex);
}

void BitmappedFont::Load(const char * image, int width, int height)
{
	w = width;
	h = height;

	SDL_Surface * temp = SDL_LoadBMP(image);
	SDL_SetColorKey(temp, SDL_TRUE, SDL_MapRGB(temp->format, 0xFF, 0x00, 0xFF));
	tex = SDL_CreateTextureFromSurface(gRenderer, temp);
	SDL_FreeSurface(temp);

	int a , tw, th;
	Uint32 format;

	SDL_QueryTexture(tex, &format, &a, &tw, &th);

	chars_per_line = tw / w;

}
void BitmappedFont::DrawString(const char * str, int x, int y, bool centred)
{
	SDL_Rect src;
	SDL_Rect dest;

	dest.x = x;
	dest.y = y;
	dest.w = w;
	dest.h = h;

	int len = SDL_strlen(str);

	if (centred)
	{
		dest.y -= (h / 2);
		int tw = (len * w) / 2;
		dest.x -= tw;
	}

	
	for (int i = 0; i < len; i++)
	{
		int c = str[i];
		int sy = c / chars_per_line;
		int sx = c - (sy * chars_per_line);
		sy *= h;
		sx *= w;
		src.x = sx;
		src.y = sy;
		src.w = w;
		src.h = h;
		
		SDL_RenderCopy(gRenderer, tex, &src, &dest);
		dest.x += w;
	}
}