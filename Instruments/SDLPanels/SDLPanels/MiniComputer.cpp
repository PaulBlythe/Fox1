#include "MiniComputer.h"

MiniComputer::MiniComputer()
{

}

MiniComputer::~MiniComputer()
{
	progs.clear();
}

int MiniComputer::Update(float dt)
{
	if (progs.size() > 0)
	{
		int res = progs[0]->Update(dt);
		if (res > 0)
		{
			progs.erase(progs.begin());
			return res;
		}
	}
	return 0;
}

void MiniComputer::LoadProgram(MiniProg * prog)
{
	progs.push_back(prog);
}