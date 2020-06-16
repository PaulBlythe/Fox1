#pragma once

struct RWRThreat
{
	float Bearing;			// In degrees
	float Range;			// In miles
	float ThreatLevel;		// type less
	bool Identified;
	bool HasLock;
	int Type;

}t_RWRThreat;