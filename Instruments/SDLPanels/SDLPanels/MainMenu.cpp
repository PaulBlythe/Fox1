#include "MainMenu.h"
#include "f16RWRUS.h"
#include "f4_fuel_flow_panel.h"
#include "B52_Programable_keyboard.h"

extern void Swap(Panel * newpanel, int orient);
extern SDL_Renderer* gRenderer;

MainMenu::MainMenu()
{
}


MainMenu::~MainMenu()
{
	SDL_DestroyTexture(graphics);
}


void MainMenu::Load()
{
	SDL_Surface * temp = SDL_LoadBMP("Assets/mainmenu.bmp");
	SDL_SetColorKey(temp, SDL_TRUE, SDL_MapRGB(temp->format, 0xFF, 0x00, 0xFF));
	graphics = SDL_CreateTextureFromSurface(gRenderer, temp);
	SDL_FreeSurface(temp);
}

void MainMenu::Update(SDL_Event* e)
{
	//Get mouse position
	int x, y;
	SDL_GetMouseState(&x, &y);

	if (e->type == SDL_MOUSEBUTTONDOWN)
	{
		if (Check(x, y, &f16_dest))
		{
			Swap(new f16RWRUS(), 0);
		}
		if (Check(x, y, &f4_fuel_flow_dest))
		{
			Swap(new f4_fuel_flow_panel(), 0);
		}
		if (Check(x, y, &b52_prog_keys_dest))
		{
			Swap(new B52_programable_keyboard(), 1);
		}
	}

}
void MainMenu::Draw(SDL_Surface* screen)
{
	SDL_RenderCopy(gRenderer, graphics, &mm_src, &mm_src);
	SDL_RenderCopy(gRenderer, graphics, &f16_src, &f16_dest);
	SDL_RenderCopy(gRenderer, graphics, &options_src, &options_dest);
	SDL_RenderCopy(gRenderer, graphics, &f4_fuel_flow_src, &f4_fuel_flow_dest);
	SDL_RenderCopy(gRenderer, graphics, &b52_prog_keys_src, &b52_prog_keys_dest);
}
void MainMenu::Register(UDPComms * comms)
{
}

bool MainMenu::Check(int x, int y, SDL_Rect * rect)
{
	if (x < rect->x)
		return false;
	if (y < rect->y)
		return false;

	if (x > rect->x + rect->w)
		return false;
	if (y > rect->y + rect->h)
		return false;

	return true;
}