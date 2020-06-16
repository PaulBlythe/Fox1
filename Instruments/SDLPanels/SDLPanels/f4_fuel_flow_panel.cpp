#include "f4_fuel_flow_panel.h"
#include "Scalars.h"

extern SDL_Renderer* gRenderer;

f4_fuel_flow_panel::f4_fuel_flow_panel()
{

}


f4_fuel_flow_panel::~f4_fuel_flow_panel()
{
	SDL_DestroyTexture(guage);
	SDL_DestroyTexture(needle);
	SDL_DestroyTexture(rpm);
	SDL_DestroyTexture(rpm_small);
}

void f4_fuel_flow_panel::Load()
{
	SDL_Surface * temp = SDL_LoadBMP("Assets/F4/F4_Fuel_Flow_Bkgnd.bmp");
	SDL_SetColorKey(temp, SDL_TRUE, SDL_MapRGB(temp->format, 0xFF, 0x00, 0xFF));
	guage = SDL_CreateTextureFromSurface(gRenderer, temp);
	SDL_FreeSurface(temp);

	temp = SDL_LoadBMP("Assets/F4/F4_RPM.bmp");
	SDL_SetColorKey(temp, SDL_TRUE, SDL_MapRGB(temp->format, 0xFF, 0x00, 0xFF));
	rpm = SDL_CreateTextureFromSurface(gRenderer, temp);
	SDL_FreeSurface(temp);

	temp = SDL_LoadBMP("Assets/F4/F4_RPM_mini_needle.bmp");
	SDL_SetColorKey(temp, SDL_TRUE, SDL_MapRGB(temp->format, 0xFF, 0x00, 0xFF));
	rpm_small = SDL_CreateTextureFromSurface(gRenderer, temp);
	SDL_FreeSurface(temp);

	temp = SDL_LoadBMP("Assets/F4/F4_G_Needle_Wht.bmp");
	SDL_SetColorKey(temp, SDL_TRUE, SDL_MapRGB(temp->format, 0xFF, 0x00, 0xFF));
	needle = SDL_CreateTextureFromSurface(gRenderer, temp);
	SDL_FreeSurface(temp);
}
void f4_fuel_flow_panel::Update(SDL_Event* e)
{
	if (e->type == SDL_MOUSEBUTTONDOWN)
	{
	}
	int t = rpm1 % 10;
	sn_angle_1 = (t / 10.0)*360.0;

	t = rpm2 % 10;
	sn_angle_2 = (t / 10.0)*360.0;

	rpm_angle_1 = GuageScale(rpm1, RPMValues, RPMRanges, 12);
	rpm_angle_2 = GuageScale(rpm2, RPMValues, RPMRanges, 12);

	needle_angle_1 = GuageScale(flow1, GuageValues, GuageRanges, 15);
	needle_angle_2 = GuageScale(flow2, GuageValues, GuageRanges, 15);
}
void f4_fuel_flow_panel::Draw(SDL_Surface* screen)
{
	SDL_RenderCopy(gRenderer, rpm, NULL, &rpm_dest_1);
	SDL_RenderCopy(gRenderer, rpm, NULL, &rpm_dest_2);
	SDL_RenderCopy(gRenderer, guage, NULL, &guage_dest_1);
	SDL_RenderCopy(gRenderer, guage, NULL, &guage_dest_2);

	SDL_SetTextureBlendMode(needle, SDL_BLENDMODE_BLEND);
	SDL_RenderCopyEx(gRenderer, needle, NULL, &needle_dest_1, needle_angle_1, &needle_c, SDL_FLIP_NONE);
	SDL_RenderCopyEx(gRenderer, needle, NULL, &needle_dest_2, needle_angle_2, &needle_c, SDL_FLIP_NONE);

	SDL_RenderCopyEx(gRenderer, needle, NULL, &rpm_needle_dest_1, rpm_angle_1, &needle_c, SDL_FLIP_NONE);
	SDL_RenderCopyEx(gRenderer, needle, NULL, &rpm_needle_dest_2, rpm_angle_2, &needle_c, SDL_FLIP_NONE);


	SDL_SetTextureBlendMode(rpm_small, SDL_BLENDMODE_BLEND);
	SDL_RenderCopyEx(gRenderer, rpm_small, NULL, &needle_small_dest_1, sn_angle_1, &needle_c2, SDL_FLIP_NONE);
	SDL_RenderCopyEx(gRenderer, rpm_small, NULL, &needle_small_dest_2, sn_angle_2, &needle_c2, SDL_FLIP_NONE);
}

void f4_fuel_flow_panel::Register(UDPComms * comms)
{
	comms->SendString("F4 ENG 1");
	comms->SendString("want:eng1.rpmpcnt");
	comms->SendString("want:eng2.rpmpcnt");
	comms->SendString("want:eng1.fuelflowpph");
	comms->SendString("want:eng2.fuelflowpph");
}