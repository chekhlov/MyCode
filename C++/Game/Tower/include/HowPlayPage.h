#pragma once
#ifndef _HOWPLAYPAGE_H_
#define _HOWPLAYPAGE_H_
#include <memory>
#include "MainScreen.h"

// Класс экрана приложения
class HowPlayPage : public GamePage
{
private:
protected:
	std::shared_ptr<GameObject> background;
	virtual void OnClickClose();
public:

	HowPlayPage(std::shared_ptr<MainScreen> owner);
	virtual void Init() override;
	virtual ~HowPlayPage();
};


#endif