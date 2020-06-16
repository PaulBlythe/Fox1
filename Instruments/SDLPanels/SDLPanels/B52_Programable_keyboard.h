#pragma once
#include "Panel.h"
#include "BitmappedFont.h"
#include "PackedFont.h"
#include "ButtonRecord.h"
#include "MiniComputer.h"
#include "RamTest.h"
#include "RomTest.h"
#include "CPUTest.h"
#include "IMGTest.h"
#include "EndProgram.h"
#include "DelayedEvent.h"
#include "CommTest.h"

#include <SDL_mixer.h>

enum SubProcesses
{
	subSelfTest,
	SUBPROCESSES
};
enum KeyboardEvents
{
	kbNothing = 0,
	kbDim,
	kbBright,
	kbSelfTest,
	kbLampTest,
	kbCommTest,
	kbAutoTest,
	kbLampTestStep,
	kbMenu,
	kbSwitchTest,
	kbSwitch1,
	kbSwitch2,
	kbL599Toggle,
	kbR599Toggle,
	kbInit,
	kbEvents
};

enum B52Modes
{
	mMenu,
	mSelfTest,
	mLampTest,
	mSwitchTest,
	mGPSInit,
	B52MODES
};

class B52_programable_keyboard :
	public Panel
{
public:
	B52_programable_keyboard();
	virtual ~B52_programable_keyboard();

	void Load();
	void Update(SDL_Event* e);
	void Draw(SDL_Surface* screen);
	void Register(UDPComms * comms);

private:
	bool Check(int x, int y, SDL_Rect * rect);
	void ChangeMode(int mode);

	SDL_Texture* keys;
	PackedFont pfont;
	MiniComputer comp;

	byte Intensity = 255;

	int mx, my;
	int subprocess = 0;
	int lamptest = 0;

	bool click = false;
	bool pressed = false;

	float oldtime = 0;

	SDL_Rect but_off = { 0,0,148,148 };
	SDL_Rect but_on = { 152,0,148,148 };

	ButtonRecord* buttons[4][5];

	ButtonRecord bKeyPresent = { true, "KEY", "PRESNT", kbNothing };
	ButtonRecord bBrightness = { true, "BRT", "", kbBright };
	ButtonRecord bDim = { true, "DIM", "", kbDim };
	ButtonRecord bSelfTest = { true, "SELF", "TEST", kbSelfTest };
	ButtonRecord bPMEStatus = { true, "PME", "STATUS", kbNothing };
	ButtonRecord bL599 = { true, "L559", "OFF", kbL599Toggle };
	ButtonRecord bR599 = { true, "R559", "OFF", kbR599Toggle };
	ButtonRecord bInitMode = { true, "INIT", "MODE", kbInit };
	ButtonRecord bCryptoKey = { true, "CRYPTO", "KEY", kbNothing };
	ButtonRecord bGPSData = { true, "GPS", "DATA", kbNothing };
	ButtonRecord bChanSmry = { true, "CHAN", "SMRY", kbNothing };
	ButtonRecord bMsnData = { true, "MSN", "DATA", kbNothing };
	ButtonRecord bOrideOff = { true, "ORIDE", "OFF", kbNothing };
	ButtonRecord bAidingOn = { true, "AIDING", "ON", kbNothing };
	ButtonRecord bEnter = { true, "ENTER", "", kbNothing };
	ButtonRecord bShift = { true, "SHIFT", "", kbNothing };
	ButtonRecord bClear = { true, "CLEAR", "", kbNothing };
	ButtonRecord bLat = { true, "LAT", "", kbNothing };

	ButtonRecord bCpuTest = { true, "", "", kbNothing };
	ButtonRecord bImgTest = { true, "", "", kbNothing };
	ButtonRecord bRomTest = { true, "", "", kbNothing };
	ButtonRecord bRamTest = { true, "", "", kbNothing };
	ButtonRecord bLampTest = { true, "LAMP", "TEST", kbLampTest };
	ButtonRecord bSwitchTest = { true, "SWITCH", "TEST", kbSwitchTest };
	ButtonRecord bCommTest = { true, "COMM", "TEST", kbCommTest };
	ButtonRecord bAutoTest = { true, "AUTO", "TEST", kbAutoTest };
	ButtonRecord bMenu = { true, "MENU", "", kbMenu };

	ButtonRecord bEmptyTest = { true, "", "", kbLampTestStep };
	ButtonRecord bEmptyCommTest = { true, "", "", 0 };
	ButtonRecord bEmpty = { true, "", "", 0 };


	/////////////////////////////////////////////////////////////////////////
	// Publishable variables
	/////////////////////////////////////////////////////////////////////////
	bool l559Powered = false;
	bool r559Powered = false;


};