//----------------------------------------------------------------------------
//
//  Copyright (C) 2004-2014 by EMGU Corporation. All rights reserved.
//
//----------------------------------------------------------------------------

#pragma once
#ifndef EMGU_OPTIM_C_H
#define EMGU_OPTIM_C_H

#include "opencv2/core/core_c.h"
#include "opencv2/optim/optim.hpp"

CVAPI(int) cveSolveLP(const cv::Mat* Func, const cv::Mat* Constr, cv::Mat* z);

CVAPI(void) cveDenoiseTVL1(const std::vector< cv::Mat >* observations, cv::Mat* result, double lambda, int niters);
#endif