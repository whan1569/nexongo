cmake_minimum_required(VERSION 3.10)
project(TCPServer)

# Boost 경로 설정 (Boost가 설치된 경로로 변경)
set(BOOST_ROOT "C:/Users/USER/Desktop/boost_1_86_0")

# Boost 라이브러리 찾기
find_package(Boost 1.86.0 REQUIRED COMPONENTS system thread)

# 포함 디렉토리 추가
include_directories(${Boost_INCLUDE_DIRS})

# 실행 파일 생성
add_executable(server src/server.cpp)
target_link_libraries(server ${Boost_LIBRARIES})

add_executable(client src/client.cpp)
target_link_libraries(client ${Boost_LIBRARIES})
