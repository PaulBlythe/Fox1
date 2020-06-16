#include <iostream>
#include <SDL.h>
#include "f16RWRUS.h"
#include "MainMenu.h"
#include "UDPComms.h"

using namespace std;

SDL_Surface *screen = NULL;
SDL_Window* window = NULL;

//The window renderer
SDL_Renderer* gRenderer = NULL;

UDPComms* comms = NULL;

//The attributes of the screen
const int SCREEN_WIDTH = 800;
const int SCREEN_HEIGHT = 600;
const int SCREEN_BPP = 32;
SDL_Rect fullscreen = { 0,0,SCREEN_WIDTH ,SCREEN_HEIGHT };

Panel * panel;

int orientation = 0;

int main(int argc, char * argv[])
{

	if (SDL_Init(SDL_INIT_EVERYTHING) < 0)
	{
		cout << "SDL initialization failed. SDL Error: " << SDL_GetError();
	}
	else
	{
		window = SDL_CreateWindow("SDL Panels", SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED, SCREEN_WIDTH, SCREEN_HEIGHT, SDL_WINDOW_SHOWN);
		if (window == NULL)
		{
			cout << "Window could not be created! SDL_Error: %s\n" << SDL_GetError();
			return 1;
		}

		gRenderer = SDL_CreateRenderer(window, -1, SDL_RENDERER_ACCELERATED | SDL_RENDERER_PRESENTVSYNC);

		//Initialize SDL_mixer
		if (Mix_OpenAudio(44100, MIX_DEFAULT_FORMAT, 2, 2048) < 0)
		{
			printf("SDL_mixer could not initialize! SDL_mixer Error: %s\n", Mix_GetError());
		}

		//Get window surface
		screen = SDL_GetWindowSurface(window);
		panel = new MainMenu();
		panel->Load();

		comms = new UDPComms();
		comms->Start();
		
		
		//Main loop flag
		bool quit = false;

		//Event handler
		SDL_Event e;

		//While application is running
		while (!quit)
		{
			//Handle events on queue
			SDL_PollEvent(&e);
			
			//User requests quit
			if (e.type == SDL_QUIT)
			{
				quit = true;
			}
			
			if (comms->status == 10)
			{
				panel->Register(comms);
				comms->status = 11;
			}
			panel->Update(&e);

			SDL_RenderClear(gRenderer);
			panel->Draw(screen);

			//Update screen
			SDL_RenderPresent(gRenderer);

		}
		SDL_DestroyRenderer(gRenderer);
		//Destroy window
		SDL_DestroyWindow(window);

		//Quit SDL subsystems
		SDL_Quit();
		Mix_Quit();

	}

	return 0;
}

void Swap(Panel * newpanel, int orient)
{
	delete panel;

	if (orientation != orient)
	{
		SDL_DestroyRenderer(gRenderer);
		SDL_DestroyWindow(window);

		switch (orient)
		{
		case 0:
			window = SDL_CreateWindow("SDL Panels", SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED, SCREEN_WIDTH, SCREEN_HEIGHT, SDL_WINDOW_SHOWN);
			gRenderer = SDL_CreateRenderer(window, -1, SDL_RENDERER_ACCELERATED | SDL_RENDERER_PRESENTVSYNC);
			break;

		case 1:
			window = SDL_CreateWindow("SDL Panels", SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED, SCREEN_HEIGHT, SCREEN_WIDTH, SDL_WINDOW_SHOWN);
			gRenderer = SDL_CreateRenderer(window, -1, SDL_RENDERER_ACCELERATED | SDL_RENDERER_PRESENTVSYNC);
			break;

		}
		orientation = orient;
	}
	panel = newpanel;
	panel->Load();

	panel->Register(comms);
}