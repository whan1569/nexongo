#include <iostream>
#include <boost/asio.hpp>

using boost::asio::ip::tcp;

int main() {
    try {
        boost::asio::io_service io_service;
        tcp::socket socket(io_service);
        tcp::resolver resolver(io_service);
        tcp::resolver::results_type endpoints = resolver.resolve("127.0.0.1", "4321");

        boost::asio::connect(socket, endpoints);

        // 수신할 데이터 버퍼
        char reply[1024];  // 수신할 데이터를 저장할 버퍼

        // 소켓을 비동기로 읽기
        socket.async_read_some(boost::asio::buffer(reply),
            [&socket, reply](const boost::system::error_code& error, std::size_t bytes_transferred) {
                if (!error) {
                    std::cout << "Reply: " << std::string(reply, bytes_transferred) << std::endl;
                }
            });

        io_service.run();  // 비동기 작업 실행
    } catch (std::exception& e) {
        std::cerr << "Exception: " << e.what() << "\n";
    }
    return 0;
}
