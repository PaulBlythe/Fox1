#include "IMGTest.h"


IMGTest::IMGTest(float time)
{
	executionTime = time;
}

IMGTest::~IMGTest()
{

}

int IMGTest::Update(float dt)
{
	executionTime -= dt;
	if (executionTime > 0)
		return 0;

	return prIMGTestPass;
}
