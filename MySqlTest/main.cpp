#include <mysql_driver.h>
#include <mysql_connection.h>
#include <cppconn/statement.h>
#include <cppconn/prepared_statement.h>
#include <iostream>

int main() {
    try {
        // MySQL 드라이버 생성
        sql::mysql::MySQL_Driver *driver = sql::mysql::get_mysql_driver_instance();

        // 데이터베이스에 연결
        std::unique_ptr<sql::Connection> con(driver->connect("tcp://127.0.0.1:3306", "user", "password"));

        // 연결 인코딩 설정
        con->setClientOption("characterEncoding", "utf8mb4");

        // 데이터베이스 선택
        con->setSchema("testdb");

        // 쿼리 실행
        std::unique_ptr<sql::Statement> stmt(con->createStatement());
        std::unique_ptr<sql::ResultSet> res(stmt->executeQuery("SELECT 'Hello World!' AS _message"));

        // 결과 출력
        while (res->next()) {
            std::cout << res->getString("_message") << std::endl;
        }
    } catch (sql::SQLException &e) {
        std::cerr << "SQL Error: " << e.what() << std::endl;
    } catch (std::exception &e) {
        std::cerr << "Error: " << e.what() << std::endl;
    }

    // 프로그램 종료 전 대기
    std::cout << "Press Enter to continue...";
    std::cin.get();  // Enter 키 입력 대기

    return 0;
}