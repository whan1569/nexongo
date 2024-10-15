#include <iostream>
#include <boost/asio.hpp>
#include <thread>

using boost::asio::ip::tcp;

int main() {
    try {
        boost::asio::io_service io_service;
        tcp::socket socket(io_service);
        tcp::resolver resolver(io_service);
        tcp::resolver::results_type endpoints = resolver.resolve("127.0.0.1", "4321");

        boost::asio::connect(socket, endpoints);

        // 클라이언트 아이디 입력
        std::string client_id;
        std::cout << "Enter your ID: ";
        std::getline(std::cin, client_id);
        
        // 아이디 전송
        boost::asio::write(socket, boost::asio::buffer(client_id + "\n"));

        std::thread receive_thread([&]() {
            char reply[1024];  // 수신할 데이터를 저장할 버퍼
            for (;;) {
                boost::system::error_code error;
                size_t len = socket.read_some(boost::asio::buffer(reply), error);

                if (error == boost::asio::error::eof) {
                    break; // 연결 종료
                } else if (error) {
                    throw boost::system::system_error(error); // 다른 에러
                }

                std::cout << "Reply: " << std::string(reply, len) << std::endl;
            }
        });

        std::string message;
        while (std::getline(std::cin, message)) {
            boost::asio::write(socket, boost::asio::buffer(client_id + ": " + message + "\n")); // 아이디 포함
        }

        receive_thread.join();
    } catch (std::exception& e) {
        std::cerr << "Exception: " << e.what() << "\n";
    }

    return 0;
}
