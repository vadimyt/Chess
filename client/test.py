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
    print(p)
    while (1):        
        k=n.send(input().encode())
        print(k.decode())
    print(k)
    pass

main()