#pragma once
#include <vector>

enum ProgramResults
{
	prRamTestPass = 1,
	prRamTestFail,
	prRomTestPass,
	prRomTestFail,
	prCPUTestPass,
	prCPUTestFail,
	prIMGTestPass,
	prIMGTestFail,
	prEnd1,
	prEnd2,
	prCommTest1,
	prCommTest2,

	prProgramResults
};


class MiniProg
{
public:
	MiniProg() {};
	virtual ~MiniProg() {};

	virtual int Update(float dt) = 0;
};


class MiniComputer
{
public:
	MiniComputer();
	~MiniComputer();

	int Update(float dt);
	void LoadProgram(MiniProg * prog);

private:
	std::vector<MiniProg *> progs;

};

