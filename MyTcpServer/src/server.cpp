#include <boost/asio.hpp>
#include <iostream>
#include <memory>
#include <set>
#include <mutex>  // mutex 추가

using boost::asio::ip::tcp;

class ChatSession : public std::enable_shared_from_this<ChatSession> {
public:
    ChatSession(tcp::socket socket, std::set<std::shared_ptr<ChatSession>>& sessions)
        : socket_(std::move(socket)), sessions_(sessions) {}

    void start() {
        std::cout << "Client connected: " << socket_.remote_endpoint().address() << std::endl;
        sessions_.insert(shared_from_this());
        do_read();
    }

    void stop() {
        std::cout << "Client disconnected: " << socket_.remote_endpoint().address() << std::endl;
        sessions_.erase(shared_from_this());
        socket_.close();
    }

    void deliver(const std::string& message) {
        auto self(shared_from_this());
        boost::asio::async_write(socket_, boost::asio::buffer(message),
            [this, self](boost::system::error_code ec, std::size_t /*length*/) {
                if (ec) {
                    stop();
                }
            });
    }

private:
    void do_read() {
        auto self(shared_from_this());
        socket_.async_read_some(boost::asio::buffer(data_, max_length),
            [this, self](boost::system::error_code ec, std::size_t length) {
                if (!ec) {
                    std::string message(data_, length);
                    std::cout << "Received: " << message << std::endl;
                    broadcast(message);  // 다른 클라이언트들에게 메시지 전달
                    do_read();
                } else {
                    stop();  // 연결이 끊어졌을 때 처리
                }
            });
    }

    void broadcast(const std::string& message) {
        // mutex를 사용하여 thread-safe하게 세션에 접근
        std::lock_guard<std::mutex> lock(mutex_);
        for (auto it = sessions_.begin(); it != sessions_.end(); ) {
            auto session = *it;
            if (session && session != shared_from_this()) {
                session->deliver(message);
                ++it; // 다음 iterator로 이동
            } else {
                it = sessions_.erase(it); // 무효화된 세션 제거
            }
        }
    }

    tcp::socket socket_;
    std::set<std::shared_ptr<ChatSession>>& sessions_;
    enum { max_length = 1024 };
    char data_[max_length];
    static std::mutex mutex_; // mutex 선언
};

// mutex 정의
std::mutex ChatSession::mutex_;

class ChatServer {
public:
    ChatServer(boost::asio::io_context& io_context, const tcp::endpoint& endpoint)
        : acceptor_(io_context, endpoint) {
        do_accept();
    }

private:
    void do_accept() {
        acceptor_.async_accept(
            [this](boost::system::error_code ec, tcp::socket socket) {
                if (!ec) {
                    auto session = std::make_shared<ChatSession>(std::move(socket), sessions_);
                    session->start();
                }
                do_accept();
            });
    }

    tcp::acceptor acceptor_;
    std::set<std::shared_ptr<ChatSession>> sessions_; // 연결된 세션 관리
};

int main() {
    try {
        boost::asio::io_context io_context;
        tcp::endpoint endpoint(tcp::v4(), 4321);
        ChatServer server(io_context, endpoint);

        io_context.run();
    } catch (std::exception& e) {
        std::cerr << "Exception: " << e.what() << std::endl;
    }

    return 0;
}
