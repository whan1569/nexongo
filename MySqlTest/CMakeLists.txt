cmake_minimum_required(VERSION 3.10)

# 프로젝트 이름
project(MySQL_CPP_Example)

# C++ 표준 설정
set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED True)

# MySQL Connector/C++ 경로 설정
set(MYSQL_CONNECTOR_DIR "C:/Program Files/MySQL/MySQL Connector C++ 9.0")

# MySQL Connector/C++ 라이브러리 찾기
find_library(MYSQLCPP_LIB mysqlcppconn PATHS "${MYSQL_CONNECTOR_DIR}/lib/vs14")
include_directories("${MYSQL_CONNECTOR_DIR}/include/jdbc")

# 실행 파일 생성
add_executable(mysql_example main.cpp)

# 링크 라이브러리
target_link_libraries(mysql_example "${MYSQLCPP_LIB}")
