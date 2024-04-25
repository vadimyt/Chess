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
    if (p.find("error")<0):
        round_counter = 0
        n.send("/register".encode("utf-8"))
        n.send(login.encode("utf-8"))
        n.send(password.encode("utf-8"))
        n.send("/login".encode("utf-8"))
        print(login)
        n.send(login.encode("utf-8"))
        time.sleep(time_delay)
        res=(n.send(password.encode("utf-8"))).decode("utf-8")
        time.sleep(time_delay)
        print(res)
        gameloop=True
        counter_timeout = 10
        counter = 0
        while(gameloop):
            print("create : 71")
            n.send("/create".encode("utf-8"))
            time.sleep(time_delay)
            connected=True
            while(connected):
                while(True):
                    try:
                        res=n.send("/check".encode("utf-8")).decode("utf-8")
                    except:
                        pass
                    time.sleep(time_delay)
                    counter+=1
                    if (counter_timeout<counter): 
                        break
                time.sleep(3)
                try:
                    res=n.send("/leave".encode("utf-8")).decode("utf-8")
                    print("leave : 113"+res+"leaved: "+login)
                    res=n.send("/quit".encode("utf-8")).decode("utf-8")
                except:
                    pass
                connected=False
                gameloop=False
                break
    return (n.delay)

def randStr(chars = string.ascii_uppercase + string.digits, N=10): 
        return ''.join(random.choice(chars) for _ in range(N)) 

if __name__ == '__main__':
    server_port = 45245
    thread_server_port = 45241
    processes_count = 10
    first_complete = False
    second_complete = False
    account_pool = []
    for j in range(processes_count-len(account_pool)):
        #time.sleep(0.1)
        account_pool.append([randStr(N=7),randStr(N=7),j%2+1, server_port])
    for acc in account_pool:
        print(acc)
    for j in range(len(account_pool)):
        account_pool[j][3]=thread_server_port
    for acc in account_pool:
        print(acc)
    time.sleep(time_delay)
    try:
        with Pool(processes=processes_count) as pool:
            multiple_results = [pool.apply_async(f, (account_pool[i])) for i in range(len(account_pool))]
            #res = pool.apply_async(time.sleep, (time_delay,))
            threaded_server_res_array=([res.get() for res in multiple_results])
        print(threaded_server_res_array)        
        second_complete = True
    except:
        print("Second not complete")
    threaded_server_res_array_min = threaded_server_res_array[0][0]
    threaded_server_res_array_max = threaded_server_res_array[0][0]
    threaded_server_res_array_mid = 0
    for k in range(len(threaded_server_res_array)):
        for m in range(len(threaded_server_res_array[k])):
            threaded_server_res_array_mid+=threaded_server_res_array[k][m]
            if (threaded_server_res_array[k][m]>threaded_server_res_array_max):
                threaded_server_res_array_max=threaded_server_res_array[k][m]
            if (threaded_server_res_array[k][m]<threaded_server_res_array_min):
                threaded_server_res_array_min=threaded_server_res_array[k][m]
    if(second_complete):
        print("threaded_server_res_array_min: " + str(threaded_server_res_array_min))
        print("threaded_server_res_array_max: " + str(threaded_server_res_array_max))
        print("threaded_server_res_array_mid: " + str(threaded_server_res_array_mid/len(threaded_server_res_array)))