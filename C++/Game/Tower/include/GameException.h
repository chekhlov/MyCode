#pragma once

#include <exception>
#include <string>

class GameException : public std::exception
{
public:
	GameException(std::string message);
	GameException(char * message);
	virtual ~GameException();
};