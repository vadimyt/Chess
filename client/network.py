import socket

class Network:
    def __init__(self):
        self.client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        #self.server = "127.0.0.1"
        #self.server = "26.213.115.163"
        self.server = "82.179.140.18"
        self.port = 45295
        self.addr = (self.server, self.port)
        self.p = self.connect()

    def getP(self):
        return self.p

    def connect(self):
        try:
            self.client.connect(self.addr)
            return self.client.recv(1024)
        except:
            pass

    def wait(self):
        return self.client.recv(1024)
    
    def only_send(self, data):
        self.client.send(data)

    def send(self, data):
        try:
            self.client.send(data)
            return self.client.recv(1024)
        except socket.error as e:
            print(e)