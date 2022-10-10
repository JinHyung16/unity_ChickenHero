package utils

import (
	"errors"
	"sync"
)

var ErrQueueEmpty = errors.New("큐가 비었습니다.")

type Queue struct {
	items []interface{}
	s     sync.Mutex
}

func (p *Queue) Size() int {
	return len(p.items)
}

func (p *Queue) Peek() (interface{}, error) {
	if p.Size() == 0 {
		temp := 0
		return temp, ErrQueueEmpty
	}

	return p.items[0], nil
}

func (p *Queue) Push(data interface{}) {
	p.s.Lock()
	defer p.s.Unlock()

	p.items = append(p.items, data)
}

func (p *Queue) Pop() (interface{}, error) {
	p.s.Lock()
	defer p.s.Unlock()

	if p.Size() == 0 {
		return 0, ErrQueueEmpty
	}

	res := p.items[0]

	if len(p.items) > 1 {
		p.items = p.items[1:]
	} else {
		p.items = p.items[:0]
	}

	return res, nil
}

func (p *Queue) GetArr() []interface{} {
	return p.items
}
