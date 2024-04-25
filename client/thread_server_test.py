from datetime import datetime, timedelta
from multiprocessing import Pool, Process
import os
import socket
import time
import random 
import string 

class Network:
    def __init__(self, port):
        self.delay:list[float] = []
        self.client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.server = "82.179.140.18"
        self.port = port
        self.addr = (self.server, self.port)
        self.p = self.connect()

    def getP(self):
        return self.p

    def connect(self):
        try:
            start_time = time.perf_counter()
            self.client.connect(self.addr)
            res = self.client.recv(1024)        
            end_time = time.perf_counter()
            self.delay.append(end_time-start_time)
            return res
        except:
            pass

    def wait(self):
        return self.client.recv(1024)
    
    def only_send(self, data):
        self.client.send(data)

    def send(self, data):
        try:
            start_time = time.perf_counter()
            self.client.send(data)
            res = self.client.recv(1024)
            end_time = time.perf_counter()
            self.delay.append(end_time-start_time)
            return res
        except socket.error as e:
            print(e)

time_delay = 0.4

max_rounds = 2

def f(login, password, player_id, port):
    if (player_id%2==0):
        time.sleep(time_delay)
    n = Network(port)
    p = str(n.getP().decode("utf-8"))
    n.send("/register".encode("utf-8"))
    n.send(login.encode("utf-8"))
    n.send(password.encode("utf-8"))
    n.send("/login".encode("utf-8"))
    print(login)
    n.send(login.encode("utf-8"))
    res=(n.send(password.encode("utf-8"))).decode("utf-8")
    print(res)
    gameloop=True
    counter_timeout = 1000
    counter = 0
    while(gameloop):
        res=n.send("/lobby".encode("utf-8")).decode("utf-8")
        if res.find("Нет лобби") >= 0:
            print("create : 71")
            n.send("/create".encode("utf-8"))
            connected=True
            while(connected):
                time.sleep(3)
                res=n.send("/leave".encode("utf-8")).decode("utf-8")
                print("leave : 113"+res+"leaved: "+login)
                res=n.send("/quit".encode("utf-8")).decode("utf-8")
                connected=False
                gameloop=False
                break
    return (n.delay)

def randStr(chars = string.ascii_uppercase + string.digits, N=10): 
        return ''.join(random.choice(chars) for _ in range(N)) 

if __name__ == '__main__':
    server_port = 45246
    thread_server_port = 45248
    processes_count = 4
    first_complete = False
    second_complete = False
    account_pool = [["hi","5",1, server_port],
                    ["tst","1",2, server_port],
                    ["hel","0",1, server_port],
                    ["asd","pass",2, server_port]]
    for j in range(0):
        time.sleep(0.3)
        account_pool.append([randStr(N=7),randStr(N=7),j%2+1, server_port])
    for acc in account_pool:
        print(acc)
    server_res_array=[]
    with Pool(processes=4) as pool:
        multiple_results = [pool.apply_async(f, (account_pool[i])) for i in range(4)]
        print([res.get() for res in multiple_results])
    print("aboba")