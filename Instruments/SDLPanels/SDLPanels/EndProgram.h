#pragma once
#include "MiniComputer.h"
class EndProgram :
	public MiniProg
{
public:
	EndProgram(int code);
	~EndProgram();

	int Update(float dt);

private:
	int exitcode;

};

