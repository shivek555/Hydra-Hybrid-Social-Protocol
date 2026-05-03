cmake_minimum_required(VERSION 3.14)
project(HydraNode VERSION 1.0 LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

include_directories(include)

# Removed Windows-specific libraries for Cloud-Native support
file(GLOB SOURCES "src/*.cpp")

add_executable(HydraNode ${SOURCES})
