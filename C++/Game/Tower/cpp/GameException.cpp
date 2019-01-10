#include "../stdafx.h"
#include "../include/GameException.h"
#include "../helpers/ShowMessage.h"
#include "../libs/fmt/include/fmt/format.h"

GameException::GameException(char* message) : exception(message)
{
//	ShowMessage("Ошибка!", fmt::format("Обнаружено исключение!\n{0}", message));
}

GameException::GameException(std::string message) : exception(message.c_str())
{
//	ShowMessage("Ошибка!", fmt::format("Обнаружено исключение!\n{0}", message));
}

GameException::~GameException()
{
	
}
