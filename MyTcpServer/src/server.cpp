#include <boost/asio.hpp>
#include <iostream>

using boost::asio::ip::tcp;

int main() {
    try {
        boost::asio::io_context io_context;

        // 서버 소켓을 생성하고 바인딩합니다.
        tcp::acceptor acceptor(io_context, tcp::endpoint(tcp::v4(), 12345));
        std::cout << "Server is listening on port 12345..." << std::endl;

        while (true) {
            // 클라이언트 연결을 수신 대기합니다.
            tcp::socket socket(io_context);
            acceptor.accept(socket);

            // 클라이언트와 연결되면 메시지를 출력합니다.
            std::cout << "Client connected: " << socket.remote_endpoint().address() << std::endl;

            // 여기에 클라이언트와의 통신 코드를 추가합니다.
        }
    } catch (std::exception& e) {
        std::cerr << "Exception: " << e.what() << std::endl;
    }

    return 0;
}
