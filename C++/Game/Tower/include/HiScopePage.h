#pragma once
#ifndef _HISCOPEPAGE_H_
#define _HISCOPEPAGE_H_
#include <memory>
#include "MainScreen.h"

// Класс экрана приложения
class HiscopePage : public GamePage
{
private:
protected:
	std::shared_ptr<GameObject> background;

	virtual void OnClickClose();
public:

	HiscopePage(std::shared_ptr<MainScreen> ownery);
	virtual void Init() override;
	virtual ~HiscopePage();
};


#endif