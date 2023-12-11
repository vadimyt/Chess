from time import sleep
from network import Network

class Figure:
    def __init__(self,x,y,type,color):
        self.x=x
        self.y=y
        self.type=type
        self.color=color
    
    def move(self,x,y):
        self.x=x
        self.y=y

def main():
    n = Network()    
    p= str(n.getP().decode("utf-8"))
    i = True
    listening = True
    gaming = True
    print(p)    
    while (1):
        inputs=input().encode()        
        k=n.send(inputs)
        if (k.decode("utf-8").find("Goodbye") >= 0):
            break
        if ((k.decode("utf-8").find("первый игрок") >= 0) and (k.decode("utf-8").find("Номер") < 0)):
            gaming=True
            i=True
            print(k.decode("utf-8"))
            while(gaming):
                if(i==True):
                    print("sending\n") 
                    k=n.send(input().encode())                    
                    if (k.decode("utf-8").find("win") >= 0):
                        break
                    print(k.decode("utf-8"))
                    n.only_send("/check".encode())
                    i=False
                else:
                    print("listening\n") 
                    while(listening):
                        k=n.wait()
                        if(k.decode("utf-8").find("alive") >=0):
                            n.only_send("alive".encode())
                        else:
                            if(k.decode("utf-8").find("Ожидаем оппонента") >=0):
                                gaming=False
                                break                                    
                            else:
                                if(k.decode("utf-8").find("lose") >=0):
                                    gaming=False
                                    break
                                else:
                                    print(k.decode("utf-8"))
                                    i=True
                                    break
        if ((k.decode("utf-8").find("второй игрок") >= 0) and (k.decode("utf-8").find("Номер") < 0)):
            gaming=True
            i=True
            print(k.decode("utf-8"))
            while(gaming):
                if(i==True):
                    print("listening\n") 
                    while(listening):
                        k=n.wait()
                        if(k.decode("utf-8").find("alive") >=0):
                            n.only_send("alive".encode())
                        else:
                            if(k.decode("utf-8").find("Ожидаем оппонента") >=0):
                                gaming=False
                                break                                    
                            else:
                                if(k.decode("utf-8").find("lose") >=0):
                                    gaming=False
                                    break
                                else:
                                    print(k.decode("utf-8"))
                                    i=False
                                    break          
                else:
                    print("sending\n") 
                    k=n.send(input().encode())
                    if (k.decode("utf-8").find("win") >= 0):
                        break
                    print(k.decode("utf-8"))
                    n.only_send("/check".encode())
                    i=True
        print(k.decode("utf-8"))
    pass

main()