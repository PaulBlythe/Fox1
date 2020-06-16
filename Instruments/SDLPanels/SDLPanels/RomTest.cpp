#include "RomTest.h"

RomTest::RomTest(float time)
{
	executionTime = time;
}

RomTest::~RomTest()
{

}

int RomTest::Update(float dt)
{
	executionTime -= dt;
	if (executionTime > 0)
		return 0;

	return prRomTestPass;
}