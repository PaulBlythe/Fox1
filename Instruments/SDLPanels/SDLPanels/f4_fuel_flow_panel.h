#pragma once
#include "Panel.h"
#include "BitmappedFont.h"
#include "PackedFont.h"
#include <SDL_mixer.h>
#include "TargetTypes.h"

class f4_fuel_flow_panel :
	public Panel
{
public:
	f4_fuel_flow_panel();
	virtual ~f4_fuel_flow_panel();

	void Load();
	void Update(SDL_Event* e);
	void Draw(SDL_Surface* screen);
	void Register(UDPComms * comms);

private:

	SDL_Texture* guage;
	SDL_Texture* needle;
	SDL_Texture* rpm;
	SDL_Texture* rpm_small;

	SDL_Rect guage_src = { 0,0,302,302 };
	SDL_Rect guage_dest_1 = { 65,-1,302,302 };
	SDL_Rect guage_dest_2 = { 433,-1,302,302 };
	SDL_Rect needle_src = { 0,0,11,110 };
	SDL_Rect needle_small_src = { 0,0,11,62 };
	SDL_Rect rpm_dest_1 = { 65,299,302,302 };
	SDL_Rect rpm_dest_2 = { 433,299,302,302 };
	SDL_Rect needle_dest_1 = { 60 + 151, 40,11,110 };
	SDL_Rect needle_dest_2 = { 433 - 5 + 151, 40,11,110 };
	SDL_Rect rpm_needle_dest_1 = { 60 + 151, 299+40,11,110 };
	SDL_Rect rpm_needle_dest_2 = { 433 - 5 + 151, 299+40,11,110 };

	SDL_Rect needle_small_dest_1 = { 60 + 120, 299 + 64,11,62 };
	SDL_Rect needle_small_dest_2 = { 428 + 120, 299 + 64,11,62 };

	SDL_Point needle_c = { 6,109 };
	SDL_Point needle_c2 = { 6, 31 };

	double needle_angle_1 = 0;
	double needle_angle_2 = 0;
	double flow1 = 1000;
	double flow2 = 9000;
	double sn_angle_1 = 0;
	double sn_angle_2 = 0;
	double rpm_angle_1 = 0;
	double rpm_angle_2 = 0;

	int rpm1 = 50;
	int rpm2 = 100;

	double GuageValues[15] = { 0,200,400,600,800,1000,1500,2000,2500,3000,4000,6000,8000,10000,12000 };
	double GuageRanges[15] = { 0,14,27.5,41,54.5,67.5,107,147.25,193.75,242.5,255.25,270,283,296,310.5 };
	double RPMValues[12] = { 0,10,20,30,40,50,60,70,80,90,100,110 };
	double RPMRanges[12] = { 0,26.5,54.5,81.75,109.5,136.75,163.5,191,217.5,244.5,271,296.5 };
};