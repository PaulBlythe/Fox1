#include "CommTest.h"

CommTest::CommTest()
{
	executionTime = 0;
}

CommTest::~CommTest()
{

}

int CommTest::Update(float dt)
{
	executionTime += dt;
	if (executionTime > 10)
		return prCommTest2;
	return 0;
}
