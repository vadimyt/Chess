k1="Игра началась с vadimyt, вы первый игрок"
k2="Игра началась с tst, вы второй игрок"

print(str(k1.find("второй")) + "\n")
print(str(k1.find("первый")) + "\n")
print(str(k2.find("второй")) + "\n")
print(str(k2.find("первый")) + "\n")

        if (k.decode("utf-8").find("первый")>0):
            while(1):
                if (i):
                    print("1")
                    n.only_send(input().encode())
                    k=n.wait()
                    print(k.decode("utf-8"))
                    i=False
                else:
                    print("2")
                    k=n.wait()
                    print(k.decode("utf-8"))
                    n.only_send(input().encode())
                    i=True
        if (k.decode("utf-8").find("второй")>0):
            while(1):
                if(i):
                    print("3")
                    k=n.wait()
                    print(k.decode("utf-8"))
                    n.only_send(input().encode())
                    i=False
                else:
                    print("4")
                    n.only_send(input().encode())
                    k=n.wait()
                    print(k.decode("utf-8"))
                    i=True