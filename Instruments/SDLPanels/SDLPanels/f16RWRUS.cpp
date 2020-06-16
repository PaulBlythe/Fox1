#include "f16RWRUS.h"
#include "RWRThreat.h"
#include "TargetTypes.h"
#include "Constants.h"
#include <iostream>

extern const char * ThreatTypeToStringMappingTable[];
extern const char * TargetTypeToStringMappingTable[];

struct RWRThreat threats[16] =
{
	{ 0,  20, 600,true, true,  TARGET_MIG_25 },
	{ 30, 25, 500,true, false, TARGET_MIG_25 },
	{ 270,20, 400,true, false, TARGET_AIRBORNE_SEARCH_RADAR },
	{ 165,20, 300,true, false, TARGET_UNKNOWN },
	{ 90,20, 200,true, false,  THREAT_SA_10 },
	{ 310,20,200,true, false,  THREAT_SA_10 },
	{ 310,20,200,true, false,  THREAT_SA_10 },
	{ 310,20,200,true, false,  THREAT_SA_10 },

	{ 310,20,200,true, false, THREAT_SA_10 },
	{ 310,20,200,true, false, THREAT_SA_10 },
	{ 310,20,200,true, false, THREAT_SA_10 },
	{ 310,20,200,true, false, THREAT_SA_10 },
	{ 310,20,200,true, false, THREAT_SA_10 },
	{ 310,20,200,true, false, THREAT_SA_10 },
	{ 310,20,200,true, false, THREAT_SA_10 },
	{ 310,20,200,true, false, THREAT_SA_10 }
};

int nThreats = 6;

extern SDL_Renderer* gRenderer;



f16RWRUS::f16RWRUS()
{
	threat_audio_channel = -1;
}

///********************************************************************************
///**  Register required comms packets											***
///********************************************************************************
void f16RWRUS::Register(UDPComms * comms)
{
	comms->SendString("F16 RWR US");
	comms->SendString("want:rwr.16");
	comms->SendString("want:missile");

}

f16RWRUS::~f16RWRUS()
{
	SDL_DestroyTexture(rwr_back);
	SDL_DestroyTexture(rwr_top);
	SDL_DestroyTexture(buttons);
	SDL_DestroyTexture(panel_back);
	SDL_DestroyTexture(icons);

	Mix_FreeChunk(aMissileWarn);

	for (int i = 0; i < TARGET_COUNT; i++)
	{
		Mix_FreeChunk(airbourne_acquire[i]);
		Mix_FreeChunk(airbourne_locked[i]);
	}

	for (int i = 0; i < THREAT_TOTAL-100; i++)
	{
		Mix_FreeChunk(ground_acquire[i]);
		Mix_FreeChunk(ground_locked[i]);
	}
}

void f16RWRUS::Load()
{
	SDL_Surface * temp = SDL_LoadBMP("Assets/F16/rwr_us_backdrop.bmp");
	SDL_SetColorKey(temp, SDL_TRUE, SDL_MapRGB(temp->format, 0xFF, 0x00, 0xFF));
	rwr_back = SDL_CreateTextureFromSurface(gRenderer, temp);
	SDL_FreeSurface(temp);

	temp = SDL_LoadBMP("Assets/F16/rwr_us_overlay.bmp");
	SDL_SetColorKey(temp, SDL_TRUE, SDL_MapRGB(temp->format, 0xFF, 0x00, 0xFF));
	rwr_top = SDL_CreateTextureFromSurface(gRenderer, temp);
	SDL_FreeSurface(temp);

	temp = SDL_LoadBMP("Assets/F16/rwr_us_buttons.bmp");
	SDL_SetColorKey(temp, SDL_TRUE, SDL_MapRGB(temp->format, 0xFF, 0x00, 0xFF));
	buttons = SDL_CreateTextureFromSurface(gRenderer, temp);
	SDL_FreeSurface(temp);

	temp = SDL_LoadBMP("Assets/F16/rwr_us_panel.bmp");
	SDL_SetColorKey(temp, SDL_TRUE, SDL_MapRGB(temp->format, 0xFF, 0x00, 0xFF));
	panel_back = SDL_CreateTextureFromSurface(gRenderer, temp);
	SDL_FreeSurface(temp);

	temp = SDL_LoadBMP("Assets/F16/icons.bmp");
	SDL_SetColorKey(temp, SDL_TRUE, SDL_MapRGB(temp->format, 0xFF, 0x00, 0xFF));
	icons = SDL_CreateTextureFromSurface(gRenderer, temp);
	SDL_FreeSurface(temp);


	font.Load("Assets/Fonts/franklin.bmp",32,32);

	pfont.Load("Assets/Fonts/System32.bmp", "Assets/Fonts/system.dat");

	aMissileWarn = Mix_LoadWAV("Assets/Audio/EWR.wav");
	if (!aMissileWarn) {
		printf("Mix_LoadWAV: %s\n", Mix_GetError());
	}

	airbourne_acquire[TARGET_UNKNOWN] = Mix_LoadWAV("Assets/Audio/DogEar.wav");
	airbourne_acquire[TARGET_ATTACK] = Mix_LoadWAV("Assets/Audio/GunDishAcq.wav");
	airbourne_acquire[TARGET_AIRBORNE_SEARCH_RADAR] = Mix_LoadWAV("Assets/Audio/snowdrift.wav");
	airbourne_acquire[TARGET_EF_2000] = Mix_LoadWAV("Assets/Audio/HawkAcq.wav");
	airbourne_acquire[TARGET_TORNADO] = Mix_LoadWAV("Assets/Audio/VinsonAcq.wav");
	airbourne_acquire[TARGET_PHANTOM] = Mix_LoadWAV("Assets/Audio/F4.wav");
	airbourne_acquire[TARGET_F5_TIGER] = Mix_LoadWAV("Assets/Audio/F5.wav");
	airbourne_acquire[TARGET_TOMCAT] = Mix_LoadWAV("Assets/Audio/F14.wav");
	airbourne_acquire[TARGET_EAGLE] = Mix_LoadWAV("Assets/Audio/F15.wav");
	airbourne_acquire[TARGET_FALCON] = Mix_LoadWAV("Assets/Audio/F16.wav");
	airbourne_acquire[TARGET_HORNET] = Mix_LoadWAV("Assets/Audio/F18.wav");
	airbourne_acquire[TARGET_MIRAGE_2000] = Mix_LoadWAV("Assets/Audio/MIRAGE.wav");
	airbourne_acquire[TARGET_MIG_21] = Mix_LoadWAV("Assets/Audio/GroznyAcq.wav");
	airbourne_acquire[TARGET_MIG_23] = Mix_LoadWAV("Assets/Audio/MIG23.wav");
	airbourne_acquire[TARGET_MIG_25] = Mix_LoadWAV("Assets/Audio/MIG25.wav");
	airbourne_acquire[TARGET_MIG_29] = Mix_LoadWAV("Assets/Audio/MIG29.wav");
	airbourne_acquire[TARGET_MIG_31] = Mix_LoadWAV("Assets/Audio/MIG31.wav");
	airbourne_acquire[TARGET_RAPTOR] = Mix_LoadWAV("Assets/Audio/PerryAcq.wav");

	airbourne_locked[TARGET_UNKNOWN] = Mix_LoadWAV("Assets/Audio/MICARLock.wav");
	airbourne_locked[TARGET_ATTACK] = Mix_LoadWAV("Assets/Audio/GunDish.wav");
	airbourne_locked[TARGET_AIRBORNE_SEARCH_RADAR] = Mix_LoadWAV("Assets/Audio/VulcanLock.wav");
	airbourne_locked[TARGET_EF_2000] = Mix_LoadWAV("Assets/Audio/HawkLock.wav");
	airbourne_locked[TARGET_TORNADO] = Mix_LoadWAV("Assets/Audio/VinsonLock.wav");
	airbourne_locked[TARGET_PHANTOM] = Mix_LoadWAV("Assets/Audio/P27Lock.wav");
	airbourne_locked[TARGET_F5_TIGER] = Mix_LoadWAV("Assets/Audio/P37Lock.wav");
	airbourne_locked[TARGET_TOMCAT] = Mix_LoadWAV("Assets/Audio/P77Lock.wav");
	airbourne_locked[TARGET_EAGLE] = Mix_LoadWAV("Assets/Audio/AIM54Lock.wav");
	airbourne_locked[TARGET_FALCON] = Mix_LoadWAV("Assets/Audio/AIM120Lock.wav");
	airbourne_locked[TARGET_HORNET] = Mix_LoadWAV("Assets/Audio/AIM120CLock.wav");
	airbourne_locked[TARGET_MIRAGE_2000] = Mix_LoadWAV("Assets/Audio/RolandLock.wav");
	airbourne_locked[TARGET_MIG_21] = Mix_LoadWAV("Assets/Audio/GroznyLock.wav");
	airbourne_locked[TARGET_MIG_23] = Mix_LoadWAV("Assets/Audio/KuznecowLock.wav");
	airbourne_locked[TARGET_MIG_25] = Mix_LoadWAV("Assets/Audio/KuznecowLock.wav");
	airbourne_locked[TARGET_MIG_29] = Mix_LoadWAV("Assets/Audio/KuznecowLock.wav");
	airbourne_locked[TARGET_MIG_31] = Mix_LoadWAV("Assets/Audio/KuznecowLock.wav");
	airbourne_locked[TARGET_RAPTOR] = Mix_LoadWAV("Assets/Audio/PerryLock.wav");


	ground_acquire[THREAT_UNKNOWN -100]   = Mix_LoadWAV("Assets/Audio/DogEar.wav");
	ground_acquire[THREAT_SA_2 -100 ]     = Mix_LoadWAV("Assets/Audio/SA3acq.wav");
	ground_acquire[THREAT_SA_3 -100 ]     = Mix_LoadWAV("Assets/Audio/SA3acq.wav");
	ground_acquire[THREAT_SA_4 -100 ]     = Mix_LoadWAV("Assets/Audio/SA6acq.wav");
	ground_acquire[THREAT_SA_5 -100 ]     = Mix_LoadWAV("Assets/Audio/SA6acq.wav");
	ground_acquire[THREAT_SA_6 -100 ]     = Mix_LoadWAV("Assets/Audio/SA6acq.wav");
	ground_acquire[THREAT_SA_8 -100 ]     = Mix_LoadWAV("Assets/Audio/SA8acq.wav");
	ground_acquire[THREAT_SA_10 - 100 ]   = Mix_LoadWAV("Assets/Audio/SA10.wav");
	ground_acquire[THREAT_SA_11 - 100 ]   = Mix_LoadWAV("Assets/Audio/SA11acq.wav");
	ground_acquire[THREAT_SA_15 - 100 ]   = Mix_LoadWAV("Assets/Audio/SA15acq.wav");
	ground_acquire[THREAT_SA_17 - 100 ]   = Mix_LoadWAV("Assets/Audio/SA15acq.wav");
	ground_acquire[THREAT_SA_19 - 100 ]   = Mix_LoadWAV("Assets/Audio/SA15acq.wav");
	ground_acquire[THREAT_NIKE - 100 ]    = Mix_LoadWAV("Assets/Audio/NeustrashAcq.wav");
	ground_acquire[THREAT_HAWK - 100 ]    = Mix_LoadWAV("Assets/Audio/HawkAcq.wav");
	ground_acquire[THREAT_SEARCH - 100 ]  = Mix_LoadWAV("Assets/Audio/GunDishAcq.wav");
	ground_acquire[THREAT_HB_AAA - 100 ]  = Mix_LoadWAV("Assets/Audio/GunDishAcq.wav");
	ground_acquire[THREAT_LB_AAA - 100 ]  = Mix_LoadWAV("Assets/Audio/GunDishAcq.wav");
	ground_acquire[THREAT_AAA - 100 ]     = Mix_LoadWAV("Assets/Audio/GroznyAcq.wav");
	ground_acquire[THREAT_KSAM - 100]     = Mix_LoadWAV("Assets/Audio/MoscowAcq.wav");
	ground_acquire[THREAT_PATRIOT - 100 ] = Mix_LoadWAV("Assets/Audio/Patriot.wav");
	
	ground_locked[THREAT_UNKNOWN - 100] = Mix_LoadWAV("Assets/Audio/KuznecowLock.wav");
	ground_locked[THREAT_SA_2 - 100] = Mix_LoadWAV("Assets/Audio/SA3lock.wav");
	ground_locked[THREAT_SA_3 - 100] = Mix_LoadWAV("Assets/Audio/SA3lock.wav");
	ground_locked[THREAT_SA_4 - 100] = Mix_LoadWAV("Assets/Audio/SA6lock.wav");
	ground_locked[THREAT_SA_5 - 100] = Mix_LoadWAV("Assets/Audio/SA6lock.wav");
	ground_locked[THREAT_SA_6 - 100] = Mix_LoadWAV("Assets/Audio/SA6lock.wav");
	ground_locked[THREAT_SA_8 - 100] = Mix_LoadWAV("Assets/Audio/SA8lock.wav");
	ground_locked[THREAT_SA_10 - 100] = Mix_LoadWAV("Assets/Audio/SA8lock.wav");
	ground_locked[THREAT_SA_11 - 100] = Mix_LoadWAV("Assets/Audio/SA11lock.wav");
	ground_locked[THREAT_SA_15 - 100] = Mix_LoadWAV("Assets/Audio/SA15lock.wav");
	ground_locked[THREAT_SA_17 - 100] = Mix_LoadWAV("Assets/Audio/SA15lock.wav");
	ground_locked[THREAT_SA_19 - 100] = Mix_LoadWAV("Assets/Audio/SA15lock.wav");
	ground_locked[THREAT_NIKE - 100] = Mix_LoadWAV("Assets/Audio/NeustrashLock.wav");
	ground_locked[THREAT_HAWK - 100] = Mix_LoadWAV("Assets/Audio/HawkLock.wav");
	ground_locked[THREAT_SEARCH - 100] = Mix_LoadWAV("Assets/Audio/GunDishlock.wav");
	ground_locked[THREAT_HB_AAA - 100] = Mix_LoadWAV("Assets/Audio/GunDishlock.wav");
	ground_locked[THREAT_LB_AAA - 100] = Mix_LoadWAV("Assets/Audio/GunDishlock.wav");
	ground_locked[THREAT_AAA - 100] = Mix_LoadWAV("Assets/Audio/GroznyLock.wav");
	ground_locked[THREAT_KSAM - 100] = Mix_LoadWAV("Assets/Audio/MoscowLock.wav");
	ground_locked[THREAT_PATRIOT - 100] = Mix_LoadWAV("Assets/Audio/PerryLock.wav");
}

void f16RWRUS::Update(SDL_Event* e)
{
	int time = SDL_GetTicks();
	frame_time = time - ticks;
	ticks = time;

	//Get mouse position
	int x, y;
	SDL_GetMouseState(&x, &y);


	if (e->type == SDL_MOUSEBUTTONDOWN)
	{
		if (Check(x, y, &handoff_dest))
		{
			handoff_state = 1;
			handoff_timer = 0;
		}
		if (!debounce)
		{
			debounce = true;

			if (Check(x, y, &test_dest))
			{
				test_state ^= 1;
				if (test_state)
				{
					missile_warn_audio_channel = Mix_PlayChannel(-1, aMissileWarn, -1);
					if (missile_warn_audio_channel < 0)
					{
						printf("Mix_PlayChannel: %s\n", Mix_GetError());

					}
				}
				else {
					Mix_FadeOutChannel(missile_warn_audio_channel, 16);
				}
			}
			
			if (Check(x, y, &mode_dest))
			{
				mode_state ^= 1;
			}
			if (Check(x, y, &sep_dest))
			{
				sep_state ^= 1;
				sep_timer = 0;
			}
			if (Check(x, y, &ship_dest))
			{
				ship_state ^= 1;
			}
			if (Check(x, y, &missile_dest))
			{
				warn_test ^= 1;
				if (warn_test)
				{
					missile_warn_audio_channel = Mix_PlayChannel(-1, aMissileWarn, -1);
					if (missile_warn_audio_channel < 0)
					{
						printf("Mix_PlayChannel: %s\n", Mix_GetError());

					}
				}
				else {
					Mix_FadeOutChannel(missile_warn_audio_channel, 16);
				}
			}
		}

	}
	else if (e->type == SDL_MOUSEBUTTONUP)
	{
		debounce = false;
		if (handoff_state > 0)
		{
			if (handoff_timer < 1000)
			{
				if (DisplayMode == 0)
					DisplayMode = 1;	// diamond float
				else
				{
					DisplayMode = 0;	// normal
					if (threat_audio_channel >= 0)
						Mix_FadeOutChannel(threat_audio_channel, 16);
					threat_audio_channel = -1;
				}
				locked_target = 0;
			}
			else {
				DisplayMode = 3;	// latched
			}
			handoff_state = 0;
		}
	}
	else {
		if (handoff_state > 0)
		{
			handoff_timer += frame_time;
			if (handoff_timer > 1000)
			{
				DisplayMode = 2;	// transient

				locked_target = (handoff_timer / 1000) % nThreats;
			}
		}
		if (sep_state > 0)
		{
			sep_timer += frame_time;
			if (sep_timer >= 5000)
				sep_state = 0;
		}
	}
}

void f16RWRUS::Draw(SDL_Surface* screen)
{
	bool flash_priority = false;

	SDL_RenderCopy(gRenderer, rwr_back, NULL, &rwr_back_dest);
	SDL_RenderCopy(gRenderer, panel_back, NULL, &panel_back_dest);
	
	warning_timer += frame_time;
	if (warning_timer > 250)
	{
		warning_timer -= 250;
	}

	int ma = nThreats;
	if ((mode_state == 1) && (nThreats > 5))
	{
		ma = 5;
		flash_priority = (warning_timer>125);
	}
	
	//*****************************************************************************************
	//* Sys test display
	//*****************************************************************************************
	if (test_state == 1)
	{
		SDL_RenderCopy(gRenderer, buttons, &ship_on_on, &ship_dest);
		SDL_RenderCopy(gRenderer, buttons, &sys_test_on, &test_dest);
		SDL_RenderCopy(gRenderer, buttons, &sep_on, &sep_dest);

		SDL_RenderCopy(gRenderer, buttons, &missile_warn_on, &missile_dest);
		SDL_RenderCopy(gRenderer, buttons, &mode_on, &mode_dest);

		SDL_RenderCopy(gRenderer, buttons, &handoff_on, &handoff_dest);


		SDL_RenderCopy(gRenderer, icons, &tCross, &test_cross_pos);
		SDL_RenderCopy(gRenderer, icons, &tDot, &test_dot_1);
		SDL_RenderCopy(gRenderer, icons, &tDot, &test_dot_2);
		SDL_RenderCopy(gRenderer, icons, &tDot, &test_dot_3);
		SDL_RenderCopy(gRenderer, icons, &tDot, &test_dot_4);


		pfont.setColor(32, 255, 0);
		pfont.DrawString("SYS  SWV  0802", 800 - 256, 140, true);
		pfont.DrawString("SELF TEST", 800 - 256, 170, true);
		pfont.DrawString("LAMPS ON", 800 - 256, 400, true);
		pfont.DrawString("AUDIO ON", 800 - 256, 430, true);
	}
	else {
		SDL_RenderCopy(gRenderer, buttons, &sys_test_off, &test_dest);


		switch (ship_state)
		{
		case 0:
			SDL_RenderCopy(gRenderer, buttons, &ship_off, &ship_dest);
			break;
		case 1:
			SDL_RenderCopy(gRenderer, buttons, &ship_off_on, &ship_dest);
			break;
		case 2:
			SDL_RenderCopy(gRenderer, buttons, &ship_on_off, &ship_dest);
			break;
		case 3:
			SDL_RenderCopy(gRenderer, buttons, &ship_on_on, &ship_dest);
			break;

		}
		
		if (sep_state==0)
			SDL_RenderCopy(gRenderer, buttons, &sep_off, &sep_dest);
		else
			SDL_RenderCopy(gRenderer, buttons, &sep_on, &sep_dest);

		if ((missile_warn_state == 1) || (warn_test == 1))
		{
			warn_on = (warning_timer > 125);
		}
		else {
			warn_on = false;
		}
		if (warn_on)
		{
			SDL_RenderCopy(gRenderer, buttons, &missile_warn_on, &missile_dest);
		}
		else
			SDL_RenderCopy(gRenderer, buttons, &missile_warn_off, &missile_dest);

		if (mode_state==0)
			SDL_RenderCopy(gRenderer, buttons, &mode_off, &mode_dest);
		else
		{
			if (flash_priority)
				SDL_RenderCopy(gRenderer, buttons, &mode_both_off, &mode_dest);
			else
				SDL_RenderCopy(gRenderer, buttons, &mode_priority, &mode_dest);
		}


		if (DisplayMode == 0)
			SDL_RenderCopy(gRenderer, buttons, &handoff_off, &handoff_dest);
		else
			SDL_RenderCopy(gRenderer, buttons, &handoff_on, &handoff_dest);

		if (nThreats == 0)
		{
			SDL_RenderCopy(gRenderer, icons, &tCross, &test_cross_pos);
			SDL_RenderCopy(gRenderer, icons, &tDot, &centre_dot);
		}
		else {
			//*************************************************************************
			//* Draw targets
			//*************************************************************************
			int cur_target = 0;

			float cx = 800 - 256;
			float cy = 44 + 256;
			pfont.setColor(22, 255, 0);

			for (int i = 0; i < ma; i++)
			{
				float scaled_range = 256 * (threats[i].Range / 60.0f);
				if (sep_state > 0)
				{
					scaled_range *= 2;
				}
				float x = cx + (scaled_range * sinf(threats[i].Bearing * DegToRad));
				float y = cy - (scaled_range * cosf(threats[i].Bearing * DegToRad));
				const char * str;
				if (threats[i].Type < THREAT_UNKNOWN)
				{
					str = TargetTypeToStringMappingTable[threats[i].Type];

					int caret_x = (int)(x - 6.5f);
					int caret_y = (int)(y - 11.5f);
					SDL_Rect dc = { caret_x,caret_y,13,7 };
					SDL_RenderCopy(gRenderer, icons, &tCaret, &dc);
				}
				else {
					str = ThreatTypeToStringMappingTable[threats[i].Type - THREAT_UNKNOWN];
				}
				if ((i == locked_target)&&(DisplayMode > 0))
				{
					SDL_Rect dc = { (int)(x-32),(int)(y-25),64,64 };
					SDL_RenderCopy(gRenderer, icons, &tDiamond, &dc);
				}
				y -= 8;
				pfont.DrawString(str, (int)x, (int)y, true);
			}

			int focused_threat = threats[locked_target].Type;
			bool has_locked = threats[cur_target].HasLock;
			Mix_Chunk * cur = target_sound_effect;
			Mix_Chunk * newcur = NULL;

			if (focused_threat < THREAT_UNKNOWN)
			{
				// airbourne target
				if (has_locked)
				{
					newcur = airbourne_locked[focused_threat];
				}
				else {
					newcur = airbourne_acquire[focused_threat];
				}
			}
			else {
				// ground target
				focused_threat -= 100;
				if (has_locked)
					newcur = ground_locked[focused_threat];
				else
					newcur = ground_acquire[focused_threat];

			}

			if (newcur != cur)
			{
				if (threat_audio_channel >=0)
					Mix_FadeOutChannel(threat_audio_channel, 16);
				if (DisplayMode > 0)
					threat_audio_channel = Mix_PlayChannel(-1, newcur, -1);
			}
		}
	}
	SDL_SetTextureBlendMode(rwr_top, SDL_BLENDMODE_BLEND);
	SDL_SetTextureAlphaMod(rwr_top, 128);
	SDL_RenderCopy(gRenderer, rwr_top, NULL, &rwr_back_dest);
}

bool f16RWRUS::Check(int x, int y, SDL_Rect * rect)
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