#include "B52_Programable_keyboard.h"

extern SDL_Renderer* gRenderer;

B52_programable_keyboard::B52_programable_keyboard()
{

}

B52_programable_keyboard::~B52_programable_keyboard()
{

}

void B52_programable_keyboard::Load()
{
	SDL_Surface * temp = SDL_LoadBMP("Assets/B52/prog_keys.bmp");
	SDL_SetColorKey(temp, SDL_TRUE, SDL_MapRGB(temp->format, 0xFF, 0x00, 0xFF));
	keys = SDL_CreateTextureFromSurface(gRenderer, temp);
	SDL_FreeSurface(temp);

	pfont.Load("Assets/Fonts/System32.bmp", "Assets/Fonts/system.dat");

	buttons[0][0] = &bKeyPresent;
	buttons[1][0] = &bBrightness;
	buttons[2][0] = &bDim;
	buttons[3][0] = &bSelfTest;

	buttons[0][1] = &bEmpty;
	buttons[1][1] = &bEmpty;
	buttons[2][1] = &bEmpty;
	buttons[3][1] = &bEmpty;

	buttons[0][2] = &bPMEStatus;
	buttons[1][2] = &bEmpty;
	buttons[2][2] = &bL599;
	buttons[3][2] = &bR599;

	buttons[0][3] = &bInitMode;
	buttons[1][3] = &bCryptoKey;
	buttons[2][3] = &bGPSData;
	buttons[3][3] = &bChanSmry;

	buttons[0][4] = &bMsnData;
	buttons[1][4] = &bOrideOff;
	buttons[2][4] = &bAidingOn;
	buttons[3][4] = &bEnter;

}

void B52_programable_keyboard::Update(SDL_Event* e)
{
	float time = SDL_GetTicks();
	float dt = (time - oldtime) / 1000.0f;
	oldtime = time;

	SDL_GetMouseState(&mx, &my);
	click = false;

	if (subprocess < 1)
	{
		switch (e->type)
		{
		case SDL_MOUSEBUTTONDOWN:
			pressed = true;
			break;

		case SDL_MOUSEBUTTONUP:
			if (pressed)
			{
				click = true;
			}
			pressed = false;
			break;
		}
		if (click)
		{
			SDL_Rect dr;
			dr.w = 148;
			dr.h = 130;
			for (int y = 0; y < 5; y++)
			{
				for (int x = 0; x < 4; x++)
				{
					dr.x = (x * 150);
					dr.y = 25 + (y * 150);
					if (Check(mx, my, &dr))
					{
						switch (buttons[x][y]->Event)
						{
						case kbBright:
							if (Intensity < 255)
							{
								Intensity = min(255, Intensity + 16);
							}
							break;

						case kbDim:
							if (Intensity > 0)
							{
								Intensity = max(0, Intensity - 16);
							}
							break;

						case kbSelfTest:
							ChangeMode(mSelfTest);
							break;

						case kbLampTest:
							ChangeMode(mLampTest);
							lamptest = 1;
							break;

						case kbLampTestStep:
							lamptest++;
							if (lamptest == 5)
								ChangeMode(mSelfTest);
							break;

						case kbMenu:
							ChangeMode(mMenu);
							break;

						case kbInit:
							ChangeMode(mGPSInit);
							break;

						case kbL599Toggle:
							l559Powered = !l559Powered;
							if (l559Powered)
								bL599.Line2 = "ON";
							else
								bL599.Line2 = "OFF";
							break;

						case kbR599Toggle:
							r559Powered = !r559Powered;
							if (r559Powered)
								bR599.Line2 = "ON";
							else
								bR599.Line2 = "OFF";
							break;

						case kbAutoTest:
							subprocess = 1;

							comp.LoadProgram(new RamTest(4));	
							comp.LoadProgram(new RomTest(5));
							comp.LoadProgram(new CPUTest(3));
							comp.LoadProgram(new IMGTest(2));
							comp.LoadProgram(new EndProgram(prEnd1));
							comp.LoadProgram(new DelayedEvent(10, prEnd2));

							bRamTest.Line1 = "RAM";
							bRamTest.Line2 = " ";
							bRomTest.Line1 = "ROM";
							bRomTest.Line2 = " ";
							bCpuTest.Line1 = "CPU";
							bCpuTest.Line2 = " ";
							bImgTest.Line1 = "IMAGE";
							bImgTest.Line2 = " ";
							break;

						case kbSwitchTest:
							ChangeMode(mSwitchTest);
							break;

						case kbSwitch1:
							buttons[x][y]->Line1 = "EXIT";
							buttons[x][y]->Line2 = "";
							buttons[x][y]->Event = kbSwitch2;
							break;

						case kbSwitch2:
							ChangeMode(mSelfTest);
							break;

						case kbCommTest:
							if (subprocess == 0)
							{
								subprocess = -1;
								comp.LoadProgram(new CommTest());
								comp.LoadProgram(new DelayedEvent(10, prCommTest2));
								bEmptyCommTest.Line1 = "ACTIVE";
							}
							break;

						}
					}
				}
			}
		}
	}
	else if (subprocess > 0)
	{
		int res = comp.Update(dt);
		switch (res)
		{
		case prRamTestPass:
			bRamTest.Line2 = "PASSED";
			break;

		case prRamTestFail:
			bRamTest.Line2 = "FAILED";
			break;

		case prRomTestPass:
			bRomTest.Line2 = "PASSED";
			break;

		case prRomTestFail:
			bRomTest.Line2 = "FAILED";
			break;

		case prCPUTestPass:
			bCpuTest.Line2 = "PASSED";
			break;

		case prCPUTestFail:
			bCpuTest.Line2 = "FAILED";
			break;

		case prIMGTestPass:
			bImgTest.Line2 = "PASSED";
			break;

		case prIMGTestFail:
			bImgTest.Line2 = "FAILED";
			break;

		case prEnd1:
			subprocess = -1;
			break;

		default:
			break;
		}
	}
	if (subprocess < 0)
	{
		int res = comp.Update(dt);
		switch (res)
		{
			case prEnd2:
			{
				subprocess = 0;
				bRamTest.Line1 = "";
				bRamTest.Line2 = "";
				bRomTest.Line1 = "";
				bRomTest.Line2 = "";
				bCpuTest.Line1 = "";
				bCpuTest.Line2 = "";
				bImgTest.Line1 = "";
				bImgTest.Line2 = "";
			}
			break;

			case prCommTest1:
				bEmptyCommTest.Line1 = "PASSED";
				break;

			case prCommTest2:
				bEmptyCommTest.Line1 = "";
				break;
		}
	}

}

void B52_programable_keyboard::Draw(SDL_Surface* screen)
{
	SDL_Rect dr;
	dr.w = 148;
	dr.h = 130;

	pfont.setColor(Intensity, Intensity, Intensity);
	SDL_SetTextureColorMod(keys, Intensity, Intensity, Intensity);
	for (int y = 0; y < 5; y++)
	{
		for (int x = 0; x < 4; x++)
		{
			dr.x = (x * 150);
			dr.y = 25 + (y * 150);
			if ((pressed) && (Check(mx, my, &dr)))
			{
				SDL_RenderCopy(gRenderer, keys, &but_on, &dr);
			}
			else {
				SDL_RenderCopy(gRenderer, keys, &but_off, &dr);
			}

			if (buttons[x][y]->Large)
			{
				if (buttons[x][y]->Line2 == "")
				{
					int dx = dr.x + (dr.w / 2);
					int dy = dr.y + (dr.h / 2) - 16;
					pfont.DrawString(buttons[x][y]->Line1, dx, dy, true);

				}
				else {

					int dx = dr.x + (dr.w / 2);
					int dy = dr.y + (dr.h / 2) - 34;
					pfont.DrawString(buttons[x][y]->Line1, dx, dy, true);

					dy = dr.y + (dr.h / 2) + 2;
					pfont.DrawString(buttons[x][y]->Line2, dx, dy, true);
				}
			}
		}
	}
	if (lamptest > 0)
	{
		SDL_Rect ldr;
		ldr.w = 148 / 4;
		ldr.h = 130;

		SDL_Rect src;
		src.w = 148 / 4;
		src.h = 130;
		src.y = 0;
		src.x = 305 + ((lamptest - 1) * src.w);

		for (int y = 0; y < 5; y++)
		{
			for (int x = 0; x < 4; x++)
			{
				ldr.x = (x * 150) + (ldr.w * (lamptest - 1));
				ldr.y = 25 + (y * 150);

				SDL_RenderCopy(gRenderer, keys, &src, &ldr);
				
			}
		}
	}
}

void B52_programable_keyboard::Register(UDPComms * comms)
{
	comms->SendString("B52 PROG KEY");
}

bool B52_programable_keyboard::Check(int x, int y, SDL_Rect * rect)
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

void B52_programable_keyboard::ChangeMode(int mode)
{
	switch (mode)
	{
	case mSwitchTest:
	{
		for (int y = 0; y < 5; y++)
		{
			for (int x = 0; x < 4; x++)
			{
				ButtonRecord * br = new ButtonRecord{ true,"SWITCH","TEST",kbSwitch1 };
				buttons[x][y] = br;
			}
		}
	}
	break;

	case mLampTest:
	{
		buttons[0][0] = &bEmptyTest;
		buttons[1][0] = &bEmptyTest;
		buttons[2][0] = &bEmptyTest;
		buttons[3][0] = &bEmptyTest;

		buttons[0][1] = &bEmptyTest;
		buttons[1][1] = &bEmptyTest;
		buttons[2][1] = &bEmptyTest;
		buttons[3][1] = &bEmptyTest;

		buttons[0][2] = &bEmptyTest;
		buttons[1][2] = &bEmptyTest;
		buttons[2][2] = &bEmptyTest;
		buttons[3][2] = &bEmptyTest;

		buttons[0][3] = &bEmptyTest;
		buttons[1][3] = &bEmptyTest;
		buttons[2][3] = &bEmptyTest;
		buttons[3][3] = &bEmptyTest;

		buttons[0][4] = &bEmptyTest;
		buttons[1][4] = &bEmptyTest;
		buttons[2][4] = &bEmptyTest;
		buttons[3][4] = &bEmptyTest;
	}
	break;
	case mMenu:
	{
		buttons[0][0] = &bKeyPresent;
		buttons[1][0] = &bBrightness;
		buttons[2][0] = &bDim;
		buttons[3][0] = &bSelfTest;

		buttons[0][1] = &bEmpty;
		buttons[1][1] = &bEmpty;
		buttons[2][1] = &bEmpty;
		buttons[3][1] = &bEmpty;

		buttons[0][2] = &bPMEStatus;
		buttons[1][2] = &bEmpty;
		buttons[2][2] = &bL599;
		buttons[3][2] = &bR599;

		buttons[0][3] = &bInitMode;
		buttons[1][3] = &bCryptoKey;
		buttons[2][3] = &bGPSData;
		buttons[3][3] = &bChanSmry;

		buttons[0][4] = &bMsnData;
		buttons[1][4] = &bOrideOff;
		buttons[2][4] = &bAidingOn;
		buttons[3][4] = &bEnter;
	}
	break;

	case mGPSInit:
	{
		buttons[0][0] = &bLat;
		buttons[1][0] = &bEmpty;
		buttons[2][0] = &bEmpty;
		buttons[3][0] = &bEmpty;

		buttons[0][1] = &bEmpty;
		buttons[1][1] = &bEmpty;
		buttons[2][1] = &bEmpty;
		buttons[3][1] = &bClear;

		buttons[0][2] = &bEmpty;
		buttons[1][2] = &bEmpty;
		buttons[2][2] = &bEmpty;
		buttons[3][2] = &bEmpty;

		buttons[0][3] = &bEmpty;
		buttons[1][3] = &bEmpty;
		buttons[2][3] = &bEmpty;
		buttons[3][3] = &bEmpty;

		buttons[0][4] = &bMenu;
		buttons[1][4] = &bEmpty;
		buttons[2][4] = &bShift;
		buttons[3][4] = &bEnter;
	}
	break;

	case mSelfTest:
	{
		buttons[0][0] = &bEmpty;
		buttons[1][0] = &bBrightness;
		buttons[2][0] = &bDim;
		buttons[3][0] = &bCpuTest;

		buttons[0][1] = &bEmpty;
		buttons[1][1] = &bEmpty;
		buttons[2][1] = &bEmpty;
		buttons[3][1] = &bRomTest;

		buttons[0][2] = &bEmpty;
		buttons[1][2] = &bEmpty;
		buttons[2][2] = &bEmpty;
		buttons[3][2] = &bRamTest;

		buttons[0][3] = &bLampTest;
		buttons[1][3] = &bSwitchTest;
		buttons[2][3] = &bCommTest;
		buttons[3][3] = &bAutoTest;

		buttons[0][4] = &bMenu;
		buttons[1][4] = &bEmpty;
		buttons[2][4] = &bEmptyCommTest;
		buttons[3][4] = &bImgTest;
	}
	break;
	}
}