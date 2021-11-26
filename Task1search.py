import os


a = []
for r, _, fs in os.walk(r"C:\h\sample"):
    for f in fs:
        a.append(r+"/"+f)

# search in files
def search(q):
    res = []
    for i in a:
        with open(i, 'rb') as f:
            if bytes(q, 'utf-8') in f.read():
                res.append(i)
    return res
