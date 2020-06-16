#pragma once
#include "Panel.h"
#include "PackedFont.h"

class MainMenu :
	public Panel
{
public:
	MainMenu();
	virtual ~MainMenu();

	void Load();
	void Update(SDL_Event* e);
	void Draw(SDL_Surface* screen);
	void Register(UDPComms * comms);

private:
	bool Check(int x, int y, SDL_Rect * rect);

	SDL_Texture* graphics;


	SDL_Rect mm_src = { 2,4,187,25 };
	SDL_Rect f16_src = {3,34,207,31};
	SDL_Rect f16_dest = { 5,50,207,31 };
	SDL_Rect options_src = { 2,69,209,32 };
	SDL_Rect options_dest = { 5,550,209,32 };
	SDL_Rect f4_fuel_flow_src = { 3,100,209,31 };
	SDL_Rect f4_fuel_flow_dest = { 5,90,209,31 };
	SDL_Rect b52_prog_keys_src = { 3,135,209,31 };
	SDL_Rect b52_prog_keys_dest = { 5,130,209,31 };
};

