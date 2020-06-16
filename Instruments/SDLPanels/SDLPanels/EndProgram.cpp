#include "EndProgram.h"

EndProgram::EndProgram(int code)
{
	exitcode = code;
}

EndProgram::~EndProgram()
{
}

int EndProgram::Update(float dt)
{
	return exitcode;
}





