#pragma once
#include "Panel.h"
#include "BitmappedFont.h"
#include "PackedFont.h"
#include <SDL_mixer.h>
#include "TargetTypes.h"

class f16RWRUS :
	public Panel
{
public:
	f16RWRUS();
	virtual ~f16RWRUS();

	void Load();
	void Update(SDL_Event* e);
	void Draw(SDL_Surface* screen);
	void Register(UDPComms * comms);

private:

	bool Check(int x, int y, SDL_Rect * rect);
	bool debounce = false;
	int sep_state = 0;							// moves low threat targets out from the centre
	int missile_warn_state = 0;
	int mode_state = 0;							// RWR is in priority mode and only the five highest priority threats are displayed. Normally 16

	int ship_state = 0;							// On (green) Activate UNKNOWN mode, each unknown signals will be displayed on the RWR as a "U"
												// Flash (green) A unknown signal has been detected and not displayed on RWR while the UNKNOWN mode is inactive
												// Off No unknown signal will be displayed on RWR
												// Ship (green) Increase the priority of naval threat signals
	int test_state = 0;
	int handoff_state = 0;						// Activate another high-priortity target. The light is on, if the selected target is not the highest priority
	int locked_target = 0;
	int warn_test = 0;
	int ticks = 0;
	int frame_time = 0;
	int warning_timer = 0;
	int handoff_timer = 0;
	int sep_timer = 0;
	int missile_warn_audio_channel;
	int threat_audio_channel;
	int tracked_target = 0;

	int DisplayMode = 0;

	bool warn_on = false;
	bool priority_mode = false;

	Mix_Chunk * target_sound_effect = NULL;

	SDL_Rect handoff_off = { 0,0,100,100 };
	SDL_Rect handoff_on = { 102,0,100,100 };

	SDL_Rect missile_warn_off = { 204,0,100,100 };
	SDL_Rect missile_warn_on = { 306,0,100,100 };

	SDL_Rect mode_off = { 408,0,100,100 };
	SDL_Rect mode_on = { 0,102,100,100 };
	SDL_Rect mode_priority = { 0,305,100,100 };
	SDL_Rect mode_both_off = { 102,305,100,100 };

	SDL_Rect sep_off = { 102,102 ,100,100 };
	SDL_Rect sep_on = { 204,102 ,100,100 };

	SDL_Rect sys_test_off = { 306,102,100,100 };
	SDL_Rect sys_test_on = { 408,102,100,100 };

	SDL_Rect ship_off = { 0, 204, 100, 100 };
	SDL_Rect ship_off_on = { 102, 204, 100, 100 };
	SDL_Rect ship_on_off = { 204, 204, 100, 100 };
	SDL_Rect ship_on_on = { 306, 204, 100, 100 };

	SDL_Texture* rwr_back;
	SDL_Texture* rwr_top;
	SDL_Texture* buttons;
	SDL_Texture* panel_back;
	SDL_Texture* icons;

	SDL_Rect rwr_back_dest = { 800 - 512, 44, 512,512 };
	SDL_Rect panel_back_dest = { 0, 44, 800 - 512,512 };
	SDL_Rect panel_back_src = { 0,0,512,512 };

	SDL_Rect ship_dest = {   5,556 - 170,90,90 };
	SDL_Rect test_dest = { 100,556 - 170,90,90 };
	SDL_Rect sep_dest =  { 195,556 - 170,90,90 };

	SDL_Rect missile_dest = { 100,556 - 270,90,90 };
	SDL_Rect mode_dest = { 195,556 - 270,90,90 };

	SDL_Rect handoff_dest = { 195,556 - 370,90,90 };
	SDL_Rect test_cross_pos = { 800 - 256 - 38,44 + 256 - 40,77,80 };
	SDL_Rect test_dot_1 = { 800 - 256 - 100,44 + 256 - 100,11,11 };
	SDL_Rect test_dot_2 = { 800 - 256 + 90,44 + 256 - 100,11,11 };
	SDL_Rect test_dot_3 = { 800 - 256 - 100,44 + 256 + 90,11,11 };
	SDL_Rect test_dot_4 = { 800 - 256 + 90,44 + 256 + 90,11,11 };
	SDL_Rect centre_dot = { 800 - 256 - 2,44 + 256 - 2,5,5 };

	BitmappedFont font;
	PackedFont pfont;

	SDL_Rect tCross = { 0,0,77,80 };
	SDL_Rect tDot = { 92,5,5,5 };
	SDL_Rect tCaret = { 115,4,13,7 };
	SDL_Rect tDiamond = { 108,18,27,27 };

	Mix_Chunk *aMissileWarn = NULL;

	Mix_Chunk *airbourne_acquire[TARGET_COUNT];
	Mix_Chunk *airbourne_locked[TARGET_COUNT];

	Mix_Chunk *ground_acquire[THREAT_TOTAL - 100];
	Mix_Chunk *ground_locked[THREAT_TOTAL -100];

};

