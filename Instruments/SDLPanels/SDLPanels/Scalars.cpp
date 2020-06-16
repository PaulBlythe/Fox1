#include "Scalars.h"

double GuageScale(double value, double * ranges, double * values, int count)
{
	if (value < ranges[0])
		return 0;
	if (value >= ranges[count - 1])
	{
		return values[count - 1];
	}

	int lower = 0;
	int higher = 1;
	while (value > ranges[higher])
	{
		lower++;
		higher++;
	}
	double range = ranges[higher] - ranges[lower];
	double delta = value - ranges[lower];
	delta /= range; // 0-1
	return values[lower] + (values[higher] - values[lower])*delta;
}